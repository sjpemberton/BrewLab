﻿namespace ViewModels

open FSharp.ViewModule
open Models.Recipe
open Units
open Models

type MainViewModel() as this = 
    inherit ViewModelBase()

    let recipe = { Name = ""
                   Ingredients = List.Empty
                   Yeast = None
                   Efficiency = 75.0<percentage>
                   BoilLength = Time 60.0<minute>
                   MashLength = Time 60.0<minute>
                   Volume = 21.0<L>
                   Style = ""
                   EstimatedOriginalGravity = 1.0<sg> 
                   Bitterness = 0.0<IBU>
                   Colour = 0.0<EBC>}

    member x.Recipe 
        with get() = new RecipeViewModel(recipe)