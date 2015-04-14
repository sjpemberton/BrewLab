namespace ViewModels

open FSharp.ViewModule

[<AbstractClass>]
type LabViewModel<'t>(model:'t) as this = 
    inherit ViewModelBase()

    ///Mutable cache of the current model snapshot - used for dirty checking etc
    let mutable model = model

    abstract member UpdateModel: 't ->'t

    member x.GetModel() = 
        let newModel = this.UpdateModel model
        model <- newModel
        newModel