namespace ViewModels

open FSharp.ViewModule
open System

type ViewModelEvent = 
    | Update

[<AbstractClass>]
type LabViewModel<'t>(model : 't) as this = 
    inherit ViewModelBase()
    //let eventController = Events.RecipeEvent.Instance
   // let mutable disposable : IDisposable option = None
    let dis = Events.subscribeVM
    let mutable model = model

    let changeEvent = Event<Events.LabEvent>()
    
    interface IDisposable with
        member x.Dispose() = 
            match disposable with
            | Some d -> d.Dispose()
            | None -> ()
    
    member x.Disposable 
        with get () = disposable
        and set (v) = disposable <- v
    
    abstract UpdateModel : 't -> 't
    abstract processUpdate : 'e -> unit
    override x.processUpdate e = ()
    //member x.Event = eventController
    //member x.onChange = eventController.Publish Events.RecipeChange
    member x.onChange = changeEvent.Trigger Events.RecipeChange
    member x.UpdateModelSnapshot() = model <- this.UpdateModel model
    member x.GetModel() = model

    member x.ChangeEvent = changeEvent.Publish

    module Events =

        let LabEvent = Event<Events.LabEvent>()
        let asObs = LabEvent.Publish

        let vms<'t> :LabViewModel<'t> list = list.Empty
    
        let propagateEvent e = 
            LabEvent.Trigger e

        let subscribeVM (vm:LabViewModel<'t>) processUpdate = 
            let d1 = asObs.Subscribe processUpdate
            let d2 = vm.ChangeEvent.Subscribe propagateEvent
            {new IDisposable with  
                member this.Dispose() = 
                    d1.Dispose()
                    d2.Dispose()}