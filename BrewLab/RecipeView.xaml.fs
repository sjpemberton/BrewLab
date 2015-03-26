namespace Views

open FsXaml

type RecipeView = XAML<"RecipeView.xaml">

type RecipeViewController() =
    inherit UserControlViewController<RecipeView>()
