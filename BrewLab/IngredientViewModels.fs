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

    type GrainViewModel (addition) as this = 
        inherit LabViewModel<GrainAddition<kg>>(addition, Events.LabEvent.FermentableChange)

        let weight = this.Factory.Backing(<@ this.Weight @>, 0.001<kg>, greaterThan (fun a -> 0.000<kg>))
        let grain = this.Factory.Backing(<@ this.Grain @>, addition.Grain)

        let GetCurrent = {Grain = grain.Value; Weight = weight.Value }

        do base.BindEvent(Events.RecipeEvent.Instance.Event, subscribe, OnLabEvent)

        override x.UpdateModel(model) = 
            { model with Grain = grain.Value; Weight = weight.Value }

        member x.GetCurrentModel() = GetCurrent
        member x.Grain with get() = grain.Value and set(v) = grain.Value <- v
        member x.Weight with get () = weight.Value and set (value) = weight.Value <- value
   

    type HopViewModel (addition, og, volume) as this = 
        inherit LabViewModel<HopAddition<g>>(addition, Events.LabEvent.HopChange)

        let mutable recipeGravity = og
        let mutable recipeVolume = volume

        let hop = this.Factory.Backing(<@ this.Hop @>, addition.Hop)
        let weight = this.Factory.Backing(<@ this.Weight @>, 0.1<g>, greaterThan (fun a -> 0.000<g>))
        let ``type`` = this.Factory.Backing(<@ this.Type @>, addition.Type)
        let time = this.Factory.Backing(<@ this.Time @>, 0.1<minute>, greaterThan (fun a -> 0.000<minute>))
        let ibu = this.Factory.Backing(<@ this.IBU @>, 0.0<IBU>)

        let GetCurrent = {Hop = hop.Value; Weight = weight.Value; Time = time.Value; Type=``type``.Value }

        do base.BindEvent(Events.RecipeEvent.Instance.Event, subscribe, OnLabEvent)

        override x.UpdateModel(model) = 
            let updated = { model with Hop = hop.Value; Weight = weight.Value; Time = time.Value; Type=``type``.Value }
            this.IBU <- CalculateIBUs updated recipeGravity recipeVolume
            updated

        member x.Hop with get() = hop.Value and set(v) = hop.Value <- v
        member x.Time with get() = time.Value and set(v) = time.Value <- v 
        member x.Type with get() = ``type``.Value and set(v) = ``type``.Value <- v 
        member x.Weight with get () = weight.Value and set (value) = weight.Value <- value
        member x.IBU with get () = ibu.Value and private set (value) = ibu.Value <- value

        //Recipe related details
        member x.UpdateRecipeDetails(og, vol) =
            recipeGravity <- og
            recipeVolume <- volume
            this.IBU <- CalculateIBUs GetCurrent og vol
            //this.IBU
       
