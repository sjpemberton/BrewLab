namespace ViewModels

open System.Collections.ObjectModel
open System.Windows.Data
open FSharp.ViewModule
open Units
open Models.Recipe
open Models
open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols
open System.ComponentModel

type RecipeViewModel(recipe) as this = 
    inherit LabViewModel<_recipe<kg,L,degC>>(recipe)

    let grain = ObservableCollection<GrainViewModel>()

    

    let addMaltCommand = 
        this.Factory.CommandSync(fun param -> 
            let g = { Name = "Maris Otter"
                      Weight = 0.001<kg>
                      Potential = 37.0<pgpkg>
                      Colour = 4.0<EBC> }
            //recipe <- AddGrain recipe g
            this.Grain.Add(GrainViewModel(g))
            this.RefreshParts)
                                        

    member x.AddMaltCommand = addMaltCommand
    member x.Grain:ObservableCollection<GrainViewModel> = grain
    member private x.RefreshParts = 
            this.GetModel() |> ignore

    override x.UpdateModel recipe=
        grain 
        |> Seq.map (fun g -> g.GetModel()) |> Seq.toList
        |> UpdateGrain recipe
        |> EstimateOriginalGravity
        
    