using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

static class Native {
    internal const int Layered = 0x80000;
    internal delegate bool EnumProc(IntPtr h, IntPtr p);
    [DllImport("user32.dll")] internal static extern bool EnumWindows(EnumProc cb, IntPtr p);
    [DllImport("user32.dll")] internal static extern bool IsWindowVisible(IntPtr h);
    [DllImport("user32.dll")] internal static extern bool IsWindow(IntPtr h);
    [DllImport("user32.dll")] internal static extern uint GetWindowThreadProcessId(IntPtr h, out uint p);
    [DllImport("user32.dll", CharSet=CharSet.Unicode)] internal static extern int GetWindowText(IntPtr h, StringBuilder s, int n);
    [DllImport("user32.dll", EntryPoint="GetWindowLongW")] internal static extern int GetStyle(IntPtr h, int i);
    [DllImport("user32.dll", EntryPoint="SetWindowLongW", SetLastError=true)] static extern int SetStyleRaw(IntPtr h, int i, int v);
    [DllImport("kernel32.dll")] static extern void SetLastError(uint e);
    [DllImport("user32.dll", SetLastError=true)] internal static extern bool SetLayeredWindowAttributes(IntPtr h, uint key, byte alpha, uint flags);
    [DllImport("user32.dll", SetLastError=true)] internal static extern bool GetLayeredWindowAttributes(IntPtr h, out uint key, out byte alpha, out uint flags);
    internal static void SetStyle(IntPtr h, int value) {
        SetLastError(0);
        int old = SetStyleRaw(h, -20, value);
        int error = Marshal.GetLastWin32Error();
        if (old == 0 && error != 0) throw new Win32Exception(error);
    }
}
sealed class WindowItem {
    internal IntPtr Handle;
    internal uint Pid;
    internal string Title;
    public override string ToString() { return Title; }
}
sealed class Original {
    internal WindowItem Window;
    internal bool Layered;
    internal uint Key, Flags;
    internal byte Alpha;
}
sealed class OpacityController {
    readonly Dictionary<IntPtr, Original> originals = new Dictionary<IntPtr, Original>();
    internal static bool Alive(WindowItem w) {
        uint pid;
        return Native.IsWindow(w.Handle) && Native.GetWindowThreadProcessId(w.Handle, out pid) != 0 && pid == w.Pid;
    }
    internal void Apply(WindowItem w, int percent) {
        if (!Alive(w)) throw new InvalidOperationException("선택한 창이 닫혔습니다. 목록을 새로고침하세요.");
        Original o;
        if (!originals.TryGetValue(w.Handle, out o) || !Alive(o.Window)) {
            int style = Native.GetStyle(w.Handle, -20);
            o = new Original { Window=w, Layered=(style & Native.Layered)!=0, Alpha=255, Flags=2 };
            if (o.Layered && !Native.GetLayeredWindowAttributes(w.Handle, out o.Key, out o.Alpha, out o.Flags))
                throw new InvalidOperationException("이 창의 기존 투명 효과를 안전하게 읽을 수 없습니다.");
            originals[w.Handle] = o;
        }
        Native.SetStyle(w.Handle, Native.GetStyle(w.Handle,-20) | Native.Layered);
        uint flags = o.Layered ? o.Flags | 2u : 2u;
        if (!Native.SetLayeredWindowAttributes(w.Handle,o.Key,(byte)Math.Round(percent*255.0/100),flags))
            throw new Win32Exception(Marshal.GetLastWin32Error());
    }
    internal void RestoreAll() {
        var failures = new List<string>();
        foreach (var pair in new List<KeyValuePair<IntPtr,Original>>(originals)) {
            Original o = pair.Value;
            try {
                if (Alive(o.Window)) {
                    if (o.Layered) {
                        if (!Native.SetLayeredWindowAttributes(pair.Key,o.Key,o.Alpha,o.Flags))
                            throw new Win32Exception(Marshal.GetLastWin32Error());
                    } else Native.SetStyle(pair.Key,Native.GetStyle(pair.Key,-20) & ~Native.Layered);
                }
                originals.Remove(pair.Key);
            } catch (Exception e) { failures.Add(e.Message); }
        }
        if (failures.Count>0) throw new InvalidOperationException(string.Join("\n",failures.ToArray()));
    }
}
sealed class MainForm : Form {
    readonly ComboBox windows = new ComboBox();
    readonly TrackBar slider = new TrackBar();
    readonly Label value = new Label();
    readonly Label status = new Label();
    readonly OpacityController controller = new OpacityController();
    readonly Dictionary<IntPtr,int> levels = new Dictionary<IntPtr,int>();
    bool refreshing;
    internal MainForm() {
        Text="Chrome 투명도 조절"; ClientSize=new Size(540,300);
        FormBorderStyle=FormBorderStyle.FixedDialog; MaximizeBox=false;
        StartPosition=FormStartPosition.CenterScreen; Font=new Font("Malgun Gothic",10);
        Controls.Add(new Label {Text="투명도를 조절할 크롬 창",Left=22,Top=20,Width=400});
        windows.SetBounds(22,50,385,30); windows.DropDownStyle=ComboBoxStyle.DropDownList; Controls.Add(windows);
        var refresh=new Button {Text="새로고침",Left=417,Top=48,Width=100,Height=32}; Controls.Add(refresh);
        value.SetBounds(22,100,480,28); Controls.Add(value);
        slider.SetBounds(18,133,502,50); slider.Minimum=20; slider.Maximum=100; slider.Value=100; slider.TickFrequency=10; Controls.Add(slider);
        Controls.Add(new Label {Text="20%: 더 투명하게                              100%: 완전히 불투명",Left=22,Top=182,Width=495,Height=25});
        var restore=new Button {Text="모두 원래대로 복원",Left=22,Top=222,Width=180,Height=34}; Controls.Add(restore);
        Controls.Add(new Label {Text="프로그램을 종료하면 자동 복원됩니다.",Left=216,Top=228,Width=310,Height=28});
        status.SetBounds(22,268,495,25); Controls.Add(status);
        refresh.Click += delegate { RefreshWindows(); };
        windows.SelectedIndexChanged += delegate {
            if (refreshing) return;
            var w=windows.SelectedItem as WindowItem; int level;
            slider.Value=w!=null && levels.TryGetValue(w.Handle,out level)?level:100;
            UpdateValue();
        };
        slider.Scroll += delegate { ApplySelected(); };
        restore.Click += delegate { try { controller.RestoreAll(); levels.Clear(); slider.Value=100; UpdateValue(); status.Text="변경한 창을 복원했습니다."; } catch(Exception e) { ShowError(e); } };
        FormClosing += delegate(object sender,FormClosingEventArgs e) {
            try { controller.RestoreAll(); } catch(Exception ex) {
                if (MessageBox.Show("일부 창을 복원하지 못했습니다. 그래도 종료할까요?\n"+ex.Message,Text,MessageBoxButtons.YesNo,MessageBoxIcon.Warning)==DialogResult.No) e.Cancel=true;
            }
        };
        RefreshWindows(); UpdateValue();
    }
    void UpdateValue() { value.Text="불투명도 "+slider.Value+"%"; }
    void ShowError(Exception e) { status.Text="적용하지 못했습니다."; MessageBox.Show(e.Message,Text,MessageBoxButtons.OK,MessageBoxIcon.Warning); }
    void ApplySelected() {
        UpdateValue(); var w=windows.SelectedItem as WindowItem; if(w==null)return;
        try { controller.Apply(w,slider.Value); levels[w.Handle]=slider.Value; status.Text="선택한 크롬 창에 적용했습니다."; } catch(Exception e) { ShowError(e); }
    }
    void RefreshWindows() {
        var selected=windows.SelectedItem as WindowItem;
        refreshing=true; windows.Items.Clear();
        Native.EnumWindows(delegate(IntPtr h,IntPtr unused) {
            if(!Native.IsWindowVisible(h))return true;
            uint pid; Native.GetWindowThreadProcessId(h,out pid);
            try {
                using(var process=Process.GetProcessById((int)pid)) {
                    if(!string.Equals(process.ProcessName,"chrome",StringComparison.OrdinalIgnoreCase))return true;
                }
                var title=new StringBuilder(1024); Native.GetWindowText(h,title,title.Capacity);
                if(title.Length>0)windows.Items.Add(new WindowItem {Handle=h,Pid=pid,Title=title.ToString()});
            } catch(ArgumentException) {} catch(Win32Exception) {} catch(InvalidOperationException) {}
            return true;
        },IntPtr.Zero);
        refreshing=false;
        if(windows.Items.Count>0) {
            int index=0;
            for(int i=0;i<windows.Items.Count;i++) if(selected!=null && ((WindowItem)windows.Items[i]).Handle==selected.Handle)index=i;
            windows.SelectedIndex=index;
        }
        slider.Enabled=windows.Items.Count>0;
        status.Text=windows.Items.Count==0?"크롬을 연 다음 새로고침을 누르세요.":windows.Items.Count+"개의 크롬 창을 찾았습니다.";
    }
}
static class Program {
    [STAThread] static void Main(string[] args) {
        Application.EnableVisualStyles(); Application.SetCompatibleTextRenderingDefault(false);
        if(args.Length>0 && args[0]=="--self-test") {
            using(var form=new Form()) {
                IntPtr h=form.Handle; uint pid; Native.GetWindowThreadProcessId(h,out pid);
                var item=new WindowItem {Handle=h,Pid=pid,Title="test"};
                var control=new OpacityController(); int initial=Native.GetStyle(h,-20);
                control.Apply(item,60); uint key,flags; byte alpha;
                if(!Native.GetLayeredWindowAttributes(h,out key,out alpha,out flags) || alpha!=153)throw new Exception("Alpha check failed");
                control.RestoreAll(); if(Native.GetStyle(h,-20)!=initial)throw new Exception("Style restore failed");
                Native.SetStyle(h,initial|Native.Layered); Native.SetLayeredWindowAttributes(h,0,210,2);
                control.Apply(item,30); control.RestoreAll();
                if(!Native.GetLayeredWindowAttributes(h,out key,out alpha,out flags)||alpha!=210||flags!=2)throw new Exception("Layered restore failed");
                Native.SetStyle(h,initial);
            }
            using(var ui=new MainForm()) { var handle=ui.Handle; }
            return;
        }
        bool created;
        using(var mutex=new System.Threading.Mutex(true,"Local\\ChromeOpacityController",out created)) {
            if(!created) { MessageBox.Show("이미 실행 중입니다.","Chrome 투명도 조절"); return; }
            Application.Run(new MainForm());
        }
    }
}
