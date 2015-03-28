namespace ViewModels

open Units
open FSharp.ViewModule
open FSharp.ViewModule.Validation

type GrainViewModel(name:string, potential:float<pgp>, colour:float<EBC>) as this = 
    inherit ViewModelBase()

    let weight = this.Factory.Backing(<@ this.Weight @>, 0.001<g>, greaterThan (fun a -> 0.0<g>))
    
    member val Name = name
    member x.Weight with get() = weight.Value and set(value) = weight.Value <- value
    member x.Potential:float<pgp> = potential
    member x.Colour:float<EBC> = colour