namespace ViewModels

open Units
open Models
open FSharp.ViewModule
open FSharp.ViewModule.Validation
open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

type GrainViewModel(grain) as this = 
    inherit LabViewModel<grain<kg>>(grain)

    let weight = this.Factory.Backing(<@ this.Weight @>, 0.001<kg>, greaterThan (fun a -> 0.000<kg>))

    override x.UpdateModel(model) = 
        { model with Weight = weight.Value }

    member val Name = grain.Name
    member x.Potential : float<pgpkg> = grain.Potential
    member x.Colour : float<EBC> = grain.Colour
    
    member x.Weight 
        with get () = weight.Value
        and set (value) = weight.Value <- value