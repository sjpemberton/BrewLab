namespace ViewModels

open FSharp.ViewModule
open Models.Recipe
open Units

type MainViewModel() as this = 
    inherit ViewModelBase()

    let recipe = { Name = ""
                   Grain = List.Empty
                   Hops = List.Empty
                   Adjuncts = List.Empty
                   Yeast = None
                   Efficiency = 75.0<percentage>
                   BoilLength = 60.0<minute>
                   MashLength = 60.0<minute>
                   Volume = 21.0<L>
                   Style = ""
                   EstimatedOriginalGravity = 0.0<sg> }

    member x.Recipe 
        with get() = new RecipeViewModel(recipe)