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
    let propagateChanges e = 
        this.UpdateModelSnapshot()
        RecipeEvent.Instance.Trigger(e)

    //Map the prop change notification to a recipe change evnt and inform everybody - Warning - implementing properties that start as initially in valid will fire multiple events
    do this.BindEvent((this :> INotifyPropertyChanged).PropertyChanged |> Observable.map (fun e -> eType) , (fun o -> o :> IObservable<_>), propagateChanges)

    interface IDisposable with
        member x.Dispose() = eventHandles |> List.iter (fun d -> d.Dispose())

    abstract UpdateModel : 't -> 't
    member x.UpdateModelSnapshot() = model <- this.UpdateModel model
    member x.GetModel() = model
        
    member x.BindEvent((event:IObservable<_>), subscribe, callback) = 
        eventHandles <- (event 
                         |> subscribe 
                         |> Observable.subscribe callback)
                         :: eventHandles