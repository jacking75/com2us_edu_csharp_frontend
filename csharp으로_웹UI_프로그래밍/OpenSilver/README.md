# OpenSilver 
- [공식 사이트](https://opensilver.net/ )
- [깃허브](https://github.com/OpenSilver/OpenSilver )
- 오래전 MS가 어도비의 플래시의 대항마로 만든 웹플러그인(ActiveX) 웹UI 툴(혹은 프레임워크)로 **실버라이트**라는 것을 만들었음. 
    - 이미 몇년 전에 MS는 실버라이트를 포기했음
	- 또 플래시도 이제 역사에서 사라졌음
- OpenSilver는 아주 간단하게 설명하면 실버라이트의 오픈소스 버전이다.
- OpenSilver is a modern, plugin-free, open-source reimplementation of Silverlight
- It uses Mono for WebAssembly and Microsoft Blazor to bring back the power of C#, XAML, and .NET to client-side Web development
- UI를 XAML로 프로그래밍 할 수 있다.
    - 기존에 실버라이트로 만든 것이나 혹은 WPF로 만든 것을 빠르게 포팅할 수 있다.
	- WPF 프로그래밍에 익숙하면 아주 쉽게 OpenSilver를 사용할 수 있다.
- OpenSilver는 C# + XAML로 만든 코드를 WebAssembly를 사용하여 웹UI 개발을 할 수 있게 해준다.
  
   
## OpenSilver Gallery
- https://opensilver.net/gallery/
- [Panels & Controls](http://opensilvershowcase.azurewebsites.net/?20211012#/XAML_Controls )
- [XAML Features](http://opensilvershowcase.azurewebsites.net/?20211012#/XAML_Features )
- [.NET Framework](http://opensilvershowcase.azurewebsites.net/?20211012#/Net_Framework )
- [Client / Server](http://opensilvershowcase.azurewebsites.net/?20211012#/Client_Server )
- [Performance](http://opensilvershowcase.azurewebsites.net/?20211012#/Performance )
- [Material Design](http://opensilvershowcase.azurewebsites.net/?20211012#/Material_Design )

  
  
  
## 사용하기
- Visual Studio 2019(16.11+) 이상이 필요하다.
- Visual Studio용 애드인을 [다운로드](https://opensilver.net/download.aspx )  하고 설치한다.
- 새 프로젝트에서 OpenSilver용 템플릿을 선택한다.  
- [Getting Started](https://doc.opensilver.net/documentation/general/getting-started-tour.html )
  

## Blazor와 비교
- 좋은 점
    - WPF를 안다면 UI 배치가 훨씬 더 쉽다
	- WPF를 안다면 UI 동작에 대해서 따로 배울 필요가 거의 없다.
	- 시뮬레이터가 있어서 실제 어떻게 보일지 알수 있다.
- 아쉬운 점 
    - MS 정품이 아님
	- Visual Studio가 필수
	- 개발 시 서버 연동은 별도로 직접 만들어야 한다  



## WPF
- [데이터 그리드](https://m.blog.naver.com/julymorning4/222073855237 )
  