namespace ViewModels

open Units
open Models
open FSharp.ViewModule
open FSharp.ViewModule.Validation
open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

type GrainViewModel(addition) as this = 
    inherit LabViewModel<GrainAddition<kg>>(addition)

    let weight = this.Factory.Backing(<@ this.Weight @>, 0.001<kg>, greaterThan (fun a -> 0.000<kg>))
    //let name = this.Factory.FromFuncs(<@ this.Name @>)

    override x.UpdateModel(model) = 
        { model with Weight = weight.Value }

    member val Name = addition.Grain.Name
    member x.Potential : float<pgpkg> = addition.Grain.Potential
    member x.Colour : float<EBC> = addition.Grain.Colour
    
    member x.Weight 
        with get () = weight.Value
        and set (value) = weight.Value <- value