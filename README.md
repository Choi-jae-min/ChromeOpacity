# ChromeOpacity

크롬 밝기 조절 프로그램

1\. ChromeOpacity.exe를 더블클릭합니다.

2\. 크롬 창을 선택합니다. 목록이 비어 있으면 크롬을 열고 새로고침을 누릅니다.

3\. 슬라이더를 움직입니다. 100%는 불투명, 20%는 많이 비치는 상태입니다.

4\. '모두 원래대로 복원'을 누르거나 프로그램을 정상 종료하면 변경 전 상태로 돌아갑니다.



Windows 기본 .NET Framework를 사용하며 별도 패키지가 필요하지 않습니다.

창 전체(탭, 주소창, 페이지)에 적용되며, 창의 마우스 조작은 유지됩니다.

새 창에는 자동 적용하지 않습니다. 새로고침 후 선택하여 조절하세요.

강제 종료 시 자동 복원이 실행되지 않습니다. 이 경우 해당 크롬 창을 닫고 다시 여세요.

관리자 권한으로 실행한 크롬은 동일 권한이 필요할 수 있습니다.

Chrome 버전이나 그래픽 환경에 따라 시각적인 효과는 다를 수 있습니다.



소스: ChromeOpacity.cs

재빌드(PowerShell, 파일이 있는 폴더에서):

\& "$env:WINDIR\\Microsoft.NET\\Framework64\\v4.0.30319\\csc.exe" /nologo /target:winexe /out:ChromeOpacity.exe /reference:System.Windows.Forms.dll /reference:System.Drawing.dll ChromeOpacity.cs



사용 API 문서:

https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setlayeredwindowattributes



