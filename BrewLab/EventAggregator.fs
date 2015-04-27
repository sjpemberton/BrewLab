module EventService

open System.Collections.Generic
open System

type LabEvent = 
    | RecipeChange
    | EquipmentChange
    | UnitsChanged 

type EventController() =

    let event = Event<LabEvent>()

    member this.Event = event.Publish
    member this.Subscribe o = this.Event.Subscribe o
    member this.Publish o =
        event.Trigger o

let controller = EventController()

let subscribe o = controller.Subscribe o

let publish o = controller.Publish o

