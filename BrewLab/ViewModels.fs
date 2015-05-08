namespace ViewModels

open FSharp.ViewModule
open System
open Events
open System.ComponentModel

[<AbstractClass>]
type LabViewModel<'t>(model : 't, eType: LabEvent) as this = 
    inherit ViewModelBase()
    let mutable model = model
    let mutable eventHandles = List.empty

    let NotifyChange e =
        this.UpdateModel()
        RecipeEvent.Instance.Trigger(e)

    //Map the prop change notification to a recipe change event and inform everybody - Warning - implementing properties that start as initially in valid will fire multiple events
    do this.BindEvent((this :> INotifyPropertyChanged).PropertyChanged |> Observable.map (fun e -> eType), (fun o -> o), NotifyChange)

    interface IDisposable with
        member x.Dispose() = eventHandles |> List.iter (fun d -> d.Dispose())

    abstract GetUpdatedModel : unit -> 't
    member x.UpdateModel() = model <- this.GetUpdatedModel()

    member x.BindEvent(event, subscribe, callback) = 
        eventHandles <- (event 
                         |> subscribe 
                         |> Observable.subscribe callback)
                         :: eventHandles