namespace ViewModels

open FSharp.ViewModule
open System

type ViewModelEvent = 
    | Update

[<AbstractClass>]
type LabViewModel<'t>(model : 't, observable: IObservable<Events.LabEvent>) as this = 
    inherit ViewModelBase()

    let disposable = 
         this.observe observable
         |> Observable.subscribe this.onChange  //Implement IDisposable on VM base
        
    ///Mutable cache of the current model snapshot - used for dirty checking etc
    let mutable model = model
    
    interface IDisposable with
        member x.Dispose() = disposable.Dispose()
    
    abstract UpdateModel : 't -> 't
    abstract observe : IObservable<Events.LabEvent> -> IObservable<Events.LabEvent>
    default x.observe o = o //Pass through
    abstract onChange: _ -> unit
    member x.UpdateModelSnapshot() = model <- this.UpdateModel model
    member x.GetModel() = model