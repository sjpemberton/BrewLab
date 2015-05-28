namespace ViewModels

open Units
open Models
open FSharp.ViewModule
open FSharp.ViewModule.Validation
open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols
open System.ComponentModel
open System

[<AutoOpen>]
module IngredientViewModels =

    let subscribe source = 
            source
            |> Observable.filter (function Events.LabEvent.UnitsChanged -> true | _ -> false)

    let OnLabEvent e =  ()

    type FermentableViewModel (addition) as this = 
        inherit LabViewModel<FermentableAddition>(addition, Events.LabEvent.FermentableChange)

        let weight = this.Factory.Backing(<@ this.Weight @>, 0.1<g>, greaterThan (fun a -> 0.0<g>))
        let fermentable = this.Factory.Backing(<@ this.Fermentable @>, addition.Fermentable)

        do base.BindEvent(Events.RecipeEvent.Instance.Event, subscribe, OnLabEvent)

        override x.GetUpdatedModel() = 
            {Fermentable = fermentable.Value; Weight = weight.Value}

        member x.Fermentable with get() = fermentable.Value and set(v) = fermentable.Value <- v
        member x.Weight with get () = weight.Value and set (value) = weight.Value <- value
   

    type HopViewModel (addition, og, volume) as this = 
        inherit LabViewModel<HopAddition>(addition, Events.LabEvent.HopChange)

        let mutable recipeGravity = og
        let mutable recipeVolume = volume

        let hop = this.Factory.Backing(<@ this.Hop @>, addition.Hop)
        let weight = this.Factory.Backing(<@ this.Weight @>, 0.1<g>, greaterThan (fun a -> 0.0<g>))
        let ``type`` = this.Factory.Backing(<@ this.Type @>, addition.Type)
        let time = this.Factory.Backing(<@ this.Time @>, 0.1<minute>, greaterThan (fun a -> 0.0<minute>))
        let ibu = this.Factory.Backing(<@ this.IBU @>, 0.0<IBU>)

        do base.BindEvent(Events.RecipeEvent.Instance.Event, subscribe, OnLabEvent)

        override x.GetUpdatedModel() = 
            let updated = { Hop = hop.Value; Weight = weight.Value; Time = time.Value; Type=``type``.Value }
            ibu.Value <- CalculateIBUs updated recipeGravity recipeVolume
            updated

        member x.Hop with get() = hop.Value and set(v) = hop.Value <- v
        member x.Time with get() = time.Value and set(v) = time.Value <- v 
        member x.Type with get() = ``type``.Value and set(v) = ``type``.Value <- v 
        member x.Weight with get () = weight.Value and set (value) = weight.Value <- value
        member x.IBU = ibu.Value

        //Recipe related details
        member x.UpdateRecipeDetails(og, vol) =
            recipeGravity <- og
            recipeVolume <- volume
            ibu.Value <- CalculateIBUs (this.GetUpdatedModel()) og vol
            //this.IBU
       
