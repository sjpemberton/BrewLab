module Events

open System

type LabEvent = 
    | GravityChange
    | VolumeChange
    | HopChange
    | FermentableChange
    | EquipmentChange
    | UnitsChanged

type LabEventArg = {eventType:LabEvent; data:obj}

type RecipeEvent private() = 
    let event = Event<LabEventArg>()
    
    static let mutable instance = Lazy.Create((fun x -> new RecipeEvent()))
    static member Instance with get() = instance.Value

    member this.Event = event.Publish
    member this.Subscribe o = this.Event.Subscribe o
    member this.Trigger o = event.Trigger o

