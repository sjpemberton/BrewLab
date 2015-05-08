namespace Views

open FsXaml
open System.Windows
open System.Windows.Controls

type RecipeView = XAML<"RecipeView.xaml", true>

type RecipeViewController() =
    inherit UserControlViewController<RecipeView>()

    //Naive and not robust selection method 
    let highlightText (e:RoutedEventArgs) =
        let tb = e.OriginalSource :?> TextBox
        tb.SelectAll()
        e.Handled <- true
        

    override this.OnLoaded view = 
        view.RecipeName.GotMouseCapture.Add highlightText