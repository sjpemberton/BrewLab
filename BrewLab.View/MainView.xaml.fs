namespace Views

open FsXaml
open System.Windows
open System.Windows.Controls

type MainView = XAML<"MainView.xaml", true>

type MainViewController() =
    inherit UserControlViewController<MainView>()
