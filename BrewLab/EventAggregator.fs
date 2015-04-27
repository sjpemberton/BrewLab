module Events

open System

type LabEvent = 
    | RecipeChange
    | EquipmentChange
    | UnitsChanged 

type private RecipeEvent() =

    let event = Event<LabEvent>()

    member this.Event = event.Publish
    member this.Subscribe (o:LabEvent -> unit) = this.Event.Subscribe o
    member this.Publish o =
        event.Trigger o

let private controller = RecipeEvent()

let getObservable = controller.Event :> IObservable<LabEvent>

let subscribe (o:LabEvent -> unit) = 
    controller.Subscribe o

let publish o = controller.Publish o

