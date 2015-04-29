namespace ViewModels

open FSharp.ViewModule
open System

[<AbstractClass>]
type LabViewModel<'t>(model : 't, observe : IEvent<'e> -> IObservable<'e>) as this = 
    inherit ViewModelBase()
    let mutable model = model
    let changeEvent = new Event<'e>()
    
    let mutable disposable = 
        { new IDisposable with
              member x.Dispose() = () }
    
    do 
        let d1 = 
            Events.RecipeEvent.Instance.Event
            |> observe
            |> Observable.subscribe this.processUpdate
        
        let d2 = this.ChangeEvent.Subscribe Events.RecipeEvent.Instance.Publish
        disposable <- { new IDisposable with
                            member this.Dispose() = 
                                d1.Dispose()
                                d2.Dispose() }
    
    interface IDisposable with
        member x.Dispose() = disposable.Dispose()
    
    member x.ChangeEvent : IEvent<_> = changeEvent.Publish
    abstract UpdateModel : 't -> 't
    abstract processUpdate : 'e -> unit
    override x.processUpdate e = ()
    member x.onChange = changeEvent.Trigger Events.RecipeChange
    member x.UpdateModelSnapshot() = model <- this.UpdateModel model
    member x.GetModel() = model