namespace ViewModels

open FSharp.ViewModule
open System

type ViewModelEvent =
| Update

[<AbstractClass>]
type LabViewModel<'t>(model:'t) as this = 
    inherit ViewModelBase()

    
    ///Mutable cache of the current model snapshot - used for dirty checking etc
    let mutable model = model

    abstract member UpdateModel: 't ->'t

    member x.UpdateModelSnapshot() = 
        model <- this.UpdateModel model

    member x.GetModel() = 
        model

     