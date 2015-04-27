namespace ViewModels

open FSharp.ViewModule
open System

type ViewModelEvent =
| Update

[<AbstractClass>]
type LabViewModel<'t>(model:'t) as this = 
    inherit ViewModelBase()

    
    let disposable = EventService.subscribe this.ChangeEvent //Implement IDisposable on VM base

    ///Mutable cache of the current model snapshot - used for dirty checking etc
    let mutable model = model

    interface IDisposable with
        member x.Dispose() = 
            disposable.Dispose()
                    

    abstract member UpdateModel: 't ->'t
    abstract member ChangeEvent: EventService.LabEvent -> unit

    member x.UpdateModelSnapshot() = 
        model <- this.UpdateModel model

    member x.GetModel() = 
        model

     