namespace ViewModels

open FSharp.ViewModule

[<AbstractClass>]
type LabViewModel<'t>(model:'t, updateModel : 't -> 't) as this = 
    inherit ViewModelBase()

    ///Mutable cache of the current model snapshot - used for dirty checking etc
    let mutable model = model

    member x.GetModel() = 
        let newModel = updateModel model
        model <- newModel
        newModel