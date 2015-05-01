module Events

open System

type LabEvent = 
    | RecipeChange
    | EquipmentChange
    | UnitsChanged

type RecipeEvent private() = 
    let event = Event<LabEvent>()
    
    static let mutable instance = Lazy.Create((fun x -> new RecipeEvent()))
    static member Instance with get() = instance.Value

    member this.Event = event.Publish
    member this.Subscribe o = this.Event.Subscribe o
    member this.Publish o = event.Trigger o

