namespace ViewModels

open Units
open Models
open FSharp.ViewModule
open FSharp.ViewModule.Validation
open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

type GrainViewModel(grain : grain<kg>) as this = 
    inherit ViewModelBase()

    ///Mutable cache of the current model snapshot - used for dirty checking etc
    let mutable _grain = grain
    let weight = this.Factory.Backing(<@ this.Weight @>, 0.001<kg>, greaterThan (fun a -> 0.000<kg>))

    member x.UpdateModel() = 
        let update = { _grain with Weight = weight.Value }
        _grain <- update
        update

    member val Name = grain.Name
    member x.Potential : float<pgpkg> = grain.Potential
    member x.Colour : float<EBC> = grain.Colour
    
    member x.Weight 
        with get () = weight.Value
        and set (value) = weight.Value <- value