module EventService

open System.Collections.Generic
open System

type LabEvent = 
    | RecipeChange
    | EquipmentChange
    | UnitsChanged 

type EventController() =

    let mutable currentKey = 0

    let subscribers = Dictionary<int, IObserver<'t>>()

    interface IObservable<LabEvent> with
        member this.Subscribe(o) =
            let key = currentKey
            currentKey <- currentKey + 1
            subscribers.Add(key, o)
            { new IDisposable with 
                member this.Dispose() =
                    subscribers.Remove(key) |> ignore } 

    member this.Publish o =
        subscribers |> Seq.iter (fun s -> s.Value.OnNext o)

let controller = EventController()

let subscribe o = controller.Subscribe o

let publish o = controller.Publish o

