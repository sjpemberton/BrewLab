namespace ViewModels

open FSharp.ViewModule
open System

type observableVM<'t> = 
    abstract ChangeEvent: IEvent<'t>

module Events =

    type private RecipeEvent () = 
        let Event = Event<'t>()

    //let vms<'t> :LabViewModel<'t> list = list.Empty
    
        member x.propagateEvent e = 
            Event.Trigger e
    
        member x.asObs = Event.Publish

    let private eventPropgator = new RecipeEvent()

    let subscribeVM (vm:observableVM<'t>) (subscribe: IEvent<'t> -> IObservable<'t>) callback = 
        let d1 = eventPropgator.asObs 
                |> subscribe 
                |> Observable.subscribe callback
        let d2 = vm.ChangeEvent.Subscribe eventPropgator.propagateEvent
        {new IDisposable with  
            member this.Dispose() = 
                d1.Dispose()
                d2.Dispose()}

type ViewModelEvent = 
    | Update

[<AbstractClass>]
type LabViewModel<'t>(model : 't) as this = 
    inherit ViewModelBase()
    //let eventController = Events.RecipeEvent.Instance
    let mutable disposable : IDisposable option = None
    //let dis = Events.subscribeVM this this.processUpdate
    let mutable model = model

    let changeEvent = new Event<Events.LabEvent>()
    
//    do
//        disposable <- Some (Events.subscribeVM this this.subscribe this.processUpdate)
    
    interface IDisposable with
        member x.Dispose() = 
            //disposable.Dispose()
            match disposable with
            | Some d -> d.Dispose()
            | None -> ()

    interface observableVM<Events.LabEvent> with
        member x.ChangeEvent = changeEvent.Publish

    
    member x.Disposable 
        with get () = disposable
        and set (v) = disposable <- v
    
    
    abstract UpdateModel : 't -> 't
    abstract processUpdate : 'e -> unit
    abstract subscribe: IEvent<Events.LabEvent> -> IObservable<Events.LabEvent>
    override x.processUpdate e = 
        ()
    override x.subscribe e =
        e :> IObservable<Events.LabEvent>
    //member x.Event = eventController
    //member x.onChange = eventController.Publish Events.RecipeChange
    member x.onChange = changeEvent.Trigger Events.RecipeChange
    member x.UpdateModelSnapshot() = model <- this.UpdateModel model
    member x.GetModel() = model


   

    