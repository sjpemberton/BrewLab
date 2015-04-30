﻿namespace ViewModels

open Units
open Models
open FSharp.ViewModule
open FSharp.ViewModule.Validation
open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols
open System

[<AutoOpen>]
module Grain =

    let observe source = 
        source
        |> Observable.filter (function Events.UnitsChanged -> true 
                                                       | _ -> false)

    type GrainViewModel (addition) as this = 
        inherit LabViewModel<GrainAddition<kg>>(addition)

        let weight = this.Factory.Backing(<@ this.Weight @>, 0.001<kg>, greaterThan (fun a -> 0.000<kg>))
        let name = this.Factory.Backing(<@ this.Name @>, addition.Grain.Name)
        let potential = this.Factory.Backing(<@ this.Potential @>, addition.Grain.Potential)
        let colour = this.Factory.Backing(<@ this.Colour @>, addition.Grain.Colour)

        let switchGrainCommand = 
            this.Factory.CommandSyncParam(fun (param:obj) ->
                match param with 
                | :? Tuple<obj, bool> as t->
                    let grain = t.Item1 :?> grain<kg> //Need to stop this being a tuple
                    this.Name <- grain.Name
                    this.Potential <- grain.Potential
                    this.Colour <- grain.Colour
                | _ -> ignore())

        do
            base.BindEvents

        override x.UpdateModel(model) = 
            { model with Weight = weight.Value }

        override x.OnSubscribe source = 
            source
            |> Observable.filter (function Events.UnitsChanged -> true 
                                                          | _ -> false)
        override x.OnEvent e =
            ()

        member x.Name with get() = name.Value and private set(v) = name.Value <- v
        member x.Potential with get() = potential.Value and private set(v) = potential.Value <- v
        member x.Colour with get() = colour.Value and private set(v) = colour.Value <- v
    
        member x.Weight 
            with get () = weight.Value
            and set (value) = 
                this.UpdateModelSnapshot()
                this.onChange
                weight.Value <- value

        member x.SwitchGrainCommand = switchGrainCommand

    