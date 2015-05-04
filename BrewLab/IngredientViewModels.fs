namespace ViewModels

open Units
open Models
open FSharp.ViewModule
open FSharp.ViewModule.Validation
open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols
open System

[<AutoOpen>]
module IngredientViewModels =

    let subscribe source = 
            source
            |> Observable.filter (function Events.UnitsChanged -> true | _ -> false)

    let OnLabEvent e =  ()

    type GrainViewModel (addition) as this = 
        inherit LabViewModel<GrainAddition<kg>>(addition)

        let weight = this.Factory.Backing(<@ this.Weight @>, 0.001<kg>, greaterThan (fun a -> 0.000<kg>))
        let grain = this.Factory.Backing(<@ this.Grain @>, addition.Grain)

        let switchGrainCommand = //Need to stop this being a tuple
            this.Factory.CommandSyncParam(fun (param:(Tuple<obj, bool>)) -> 
                this.Grain <- param.Item1 :?> grain<kg>)

        do base.BindEvent(Events.RecipeEvent.Instance.Event, subscribe, OnLabEvent)

        override x.UpdateModel(model) = 
            { model with Grain = grain.Value; Weight = weight.Value }

        member x.Grain with get() = grain.Value and private set(v) = grain.Value <- v
    
        member x.Weight 
            with get () = weight.Value
            and set (value) = 
                this.UpdateModelSnapshot()
                this.onChange
                weight.Value <- value

        member x.SwitchGrainCommand = switchGrainCommand

   
    type HopViewModel (addition) as this = 
        inherit LabViewModel<HopAddition<g>>(addition)

        let hop = this.Factory.Backing(<@ this.Hop @>, addition.Hop)
        let weight = this.Factory.Backing(<@ this.Weight @>, 0.0<g>, greaterThan (fun a -> 0.000<g>))
        let ``type`` = this.Factory.Backing(<@ this.Type @>, addition.Type)
        let time = this.Factory.Backing(<@ this.Time @>, 0.0<minute>, greaterThan (fun a -> 0.000<minute>))

        let switchHopCommand = 
            this.Factory.CommandSyncParam(fun (param:(Tuple<obj, bool>)) -> 
                this.Hop <- param.Item1 :?> hop<g>)

        do base.BindEvent(Events.RecipeEvent.Instance.Event, subscribe, OnLabEvent)

        override x.UpdateModel(model) = 
            { model with Hop = hop.Value; Weight = weight.Value; Time = time.Value; Type=``type``.Value }

        member x.Hop with get() = hop.Value and set(v) = hop.Value <- v  //TODO - Map Updates
        member x.Time with get() = time.Value and private set(v) = time.Value <- v //TODO - Map Updates
        member x.Type with get() = ``type``.Value and private set(v) = ``type``.Value <- v //TODO - Map Updates
    
        member x.Weight 
            with get () = weight.Value
            and set (value) = 
                this.UpdateModelSnapshot()
                this.onChange
                weight.Value <- value

        member x.SwitchHopCommand = switchHopCommand

        type IngredientVM =
        | HopVM of HopViewModel
        | GrainVM of GrainViewModel