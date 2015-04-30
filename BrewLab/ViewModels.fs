namespace ViewModels

open FSharp.ViewModule
open System
open Events

[<AbstractClass>]
type LabViewModel<'t>(model : 't) as this = 
    inherit ViewModelBase()
    let mutable model = model
    let mutable disposable:IDisposable option = None    
    let changeEvent = new Event<LabEvent>()

    interface IDisposable with
        member x.Dispose() = 
            match disposable with
            | Some d -> d.Dispose()
            | None ->()

    abstract UpdateModel : 't -> 't
    abstract OnEvent : Events.LabEvent -> unit
    abstract OnSubscribe: IObservable<LabEvent> -> IObservable<LabEvent>
    member x.ChangeEvent : IEvent<LabEvent> = changeEvent.Publish
    member x.onChange = changeEvent.Trigger Events.RecipeChange
    member x.UpdateModelSnapshot() = model <- this.UpdateModel model
    member x.GetModel() = model
        
    member x.BindEvents = 
        let updateHandle = RecipeEvent.Instance.Event  |> this.OnSubscribe  |> Observable.subscribe this.OnEvent
        let publishHandle = this.ChangeEvent.Subscribe RecipeEvent.Instance.Publish

        disposable <- Some { new IDisposable with
                            member this.Dispose() = 
                                updateHandle.Dispose()
                                publishHandle.Dispose() }