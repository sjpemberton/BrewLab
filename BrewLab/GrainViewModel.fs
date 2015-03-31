namespace ViewModels

open Units
open Models
open FSharp.ViewModule
open FSharp.ViewModule.Validation
open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

type GrainViewModel(grain:grain<kg>) as this = 
    inherit ViewModelBase()

    let weight = this.Factory.Backing(<@ this.Weight @>, 0.001<kg>, greaterThan (fun a -> 0.000<kg>))
    
    member val Name = grain.Name
    member x.Weight with get() = weight.Value and set(value) = weight.Value <- value
    member x.Potential:float<pgpkg> = grain.Potential
    member x.Colour:float<EBC> = grain.Colour