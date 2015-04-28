namespace ViewModels

open FSharp.ViewModule
open System

type ViewModelEvent = 
    | Update

[<AbstractClass>]
type LabViewModel<'t>(model : 't) as this = 
    inherit ViewModelBase()
    let eventController = Events.RecipeEvent.Instance
    let mutable disposable : IDisposable option = None
    
    ///Mutable cache of the current model snapshot - used for dirty checking etc
    let mutable model = model
    
    interface IDisposable with
        member x.Dispose() = 
            match disposable with
            | Some d -> d.Dispose()
            | None -> ()
    
    member x.Disposable 
        with get () = disposable
        and set (v) = disposable <- v
    
    member x.Event = eventController
    member x.onChange = eventController.Publish Events.RecipeChange
    abstract processUpdate: 'e -> unit
    default x.processUpdate e = 
        ()
    member x.UpdateModelSnapshot() = model <- this.UpdateModel model
    member x.GetModel() = model
    
    abstract UpdateModel : 't -> 't