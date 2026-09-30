<div align="center">

# 🪟 ChromeOpacity

**슬라이더 하나로 조절하는 크롬 창 투명도**

크롬 너머의 화면을 함께 보세요. 원하는 창을 선택하고, 편안한 투명도로 조절하세요.

![Platform](https://img.shields.io/badge/Platform-Windows-0078D4?style=flat-square)
![Language](https://img.shields.io/badge/Language-C%23-512BD4?style=flat-square)
![Runtime](https://img.shields.io/badge/Runtime-.NET_Framework-68217A?style=flat-square)

[사용 방법](#-사용-방법) · [빌드 방법](#-빌드-방법) · [참고 사항](#-참고-사항)

</div>

---

## ✨ 주요 기능

| 기능 | 설명 |
| :--- | :--- |
| 🪟 창 선택 | 열려 있는 크롬 창 중 조절할 창을 선택합니다. |
| 🎚️ 투명도 조절 | 슬라이더로 불투명도를 **20~100%** 범위에서 조절합니다. |
| 🖱️ 마우스 조작 유지 | 투명하게 만든 창도 평소처럼 클릭하고 사용할 수 있습니다. |
| ↩️ 원래 상태로 복원 | 복원 버튼을 누르거나 프로그램을 정상 종료하면 변경 전 상태로 돌아갑니다. |
| 📦 간단한 실행 | Windows 기본 .NET Framework를 사용하며 별도 패키지 설치가 필요하지 않습니다. |

> **ChromeOpacity는 창의 투명도를 조절합니다.** 화면 밝기를 낮추는 기능과는 다르며, 탭·주소창·페이지를 포함한 창 전체에 적용됩니다.

## 🚀 사용 방법

1. **`ChromeOpacity.exe`를 실행합니다.**  
   파일을 더블클릭하면 조절 창이 열립니다.

2. **크롬 창을 선택합니다.**  
   목록이 비어 있다면 크롬을 연 뒤 **새로고침**을 누르세요.

3. **슬라이더를 움직입니다.**  
   숫자가 낮을수록 크롬 뒤의 화면이 더 많이 비칩니다.

   | 불투명도 | 화면 상태 |
   | :---: | :--- |
   | **100%** | 완전히 불투명 |
   | **60%** | 뒤의 화면이 비치는 반투명 상태 |
   | **20%** | 뒤의 화면이 많이 비치는 상태 |

20%
<img width="878" height="624" alt="image" src="https://github.com/user-attachments/assets/45a714be-39b3-4edc-8fd9-33524d7153a8" />
80%
<img width="879" height="624" alt="image" src="https://github.com/user-attachments/assets/af22619b-d03b-4003-84e7-b8163019a0a6" />
100%
<img width="872" height="627" alt="image" src="https://github.com/user-attachments/assets/2dce7d1a-389c-43db-bea3-522ef002bd23" />


4. **필요할 때 원래대로 복원합니다.**  
   **모두 원래대로 복원** 버튼을 누르거나 프로그램을 정상 종료하세요.

## 🛠️ 빌드 방법

소스 파일은 **`ChromeOpacity.cs`**입니다. 해당 파일이 있는 폴더에서 **PowerShell**을 열고 다음 명령을 실행하세요.

```powershell
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /target:winexe /out:ChromeOpacity.exe /reference:System.Windows.Forms.dll /reference:System.Drawing.dll ChromeOpacity.cs
```

빌드가 완료되면 같은 폴더에 `ChromeOpacity.exe`가 생성됩니다.

```powershell
# 프로그램 실행
.\ChromeOpacity.exe
```

> 다시 빌드할 때는 실행 중인 `ChromeOpacity.exe`를 먼저 종료하세요. 위 명령은 Windows의 64비트 .NET Framework C# 컴파일러 경로를 사용합니다.

## 📌 참고 사항

- **새 창:** 새로 연 크롬 창에는 자동 적용되지 않습니다. 새로고침 후 창을 선택해 조절하세요.
- **강제 종료:** 프로그램을 강제 종료하면 자동 복원이 실행되지 않습니다. 이 경우 해당 크롬 창을 닫고 다시 여세요.
- **실행 권한:** 관리자 권한으로 실행한 크롬을 조절하려면 이 프로그램도 동일한 권한이 필요할 수 있습니다.
- **호환성:** Chrome 버전이나 그래픽 환경에 따라 시각적인 효과가 다를 수 있습니다.
