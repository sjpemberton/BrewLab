namespace ViewModels

open FSharp.ViewModule
open System
open Events
[<AutoOpen>]
module ViewModels =

    let subscribe event observable callback = 
        event
        |> observable
        |> Observable.subscribe callback

    [<AbstractClass>]
    type LabViewModel<'t>(model : 't, observe : IEvent<'e> -> IObservable<'e>) as this = 
        inherit ViewModelBase()
        let mutable model = model
        let changeEvent = new Event<'e>()
    
        let mutable disposable = 
            { new IDisposable with
                  member x.Dispose() = () }
    
        do 
            let updateHandle = subscribe RecipeEvent.Instance.Event observe this.processUpdate
            let publishHandle = subscribe this.ChangeEvent (fun o -> o) RecipeEvent.Instance.Publish

            disposable <- { new IDisposable with
                                member this.Dispose() = 
                                    updateHandle.Dispose()
                                    publishHandle.Dispose() }
    
        interface IDisposable with
            member x.Dispose() = disposable.Dispose()
    
        member x.ChangeEvent : IEvent<_> = changeEvent.Publish
        member x.onChange = changeEvent.Trigger Events.RecipeChange
        abstract UpdateModel : 't -> 't
        abstract processUpdate : 'e -> unit
        override x.processUpdate e = ()
        member x.UpdateModelSnapshot() = model <- this.UpdateModel model
        member x.GetModel() = model