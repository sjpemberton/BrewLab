namespace ViewModels

open FSharp.ViewModule
open System
open Events

[<AbstractClass>]
type LabViewModel<'t>(model : 't) as this = 
    inherit ViewModelBase()
    let mutable model = model
    let changeEvent = new Event<LabEvent>()
    let mutable eventHandles = List.empty
    
    do this.BindEvent(this.ChangeEvent, (fun o -> o :> IObservable<_>), RecipeEvent.Instance.Publish)

    interface IDisposable with
        member x.Dispose() = eventHandles |> List.iter (fun d -> d.Dispose())

    abstract UpdateModel : 't -> 't
    member x.ChangeEvent : IEvent<LabEvent> = changeEvent.Publish
    member x.onChange = changeEvent.Trigger Events.RecipeChange
    member x.UpdateModelSnapshot() = model <- this.UpdateModel model
    member x.GetModel() = model
        
    member x.BindEvent(event, subscribe, callback) = 
        eventHandles <- (event 
                         |> subscribe 
                         |> Observable.subscribe callback)
                         :: eventHandles