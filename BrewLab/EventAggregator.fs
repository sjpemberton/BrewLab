module EventAggregator

open System.Collections.Generic

type LabEvent = 
    | RecipeChange
    | EquipmentChange
    | UnitsChanged

let private subscribers = new Dictionary<LabEvent, (LabEvent -> unit) list>()

let Subscribe event action = 
    if subscribers.ContainsKey event 
    then subscribers.[event] <- subscribers.[event] @ action

let UnSubscribe action = ()

let Publish event = 
    subscribers.[event]
    |> List.iter (fun f -> f event)