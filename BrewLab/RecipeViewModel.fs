namespace ViewModels

open System.Collections.ObjectModel
open FSharp.ViewModule
open Units

type RecipeViewModel() as this = 
    inherit ViewModelBase()
    let grain = ObservableCollection<GrainViewModel>()
    let addMaltCommand = 
        this.Factory.CommandSync
            (fun param -> 
            grain.Add(GrainViewModel(name = "Maris Otter", Weight = 0.001<g>, potential = 37.0<pgp>, colour = 4.0<EBC>)))

    member x.AddMaltCommand = addMaltCommand
    member x.Grain = grain