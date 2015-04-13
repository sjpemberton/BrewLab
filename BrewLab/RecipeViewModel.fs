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
    inherit LabViewModel<_recipe<kg,L,degC>>(recipe, this.UpdateModel)
    
    //let mutable recipe = recipe
//        { Name = ""
//          Grain = List.Empty
//          Hops = List.Empty
//          Adjuncts = List.Empty
//          Yeast = None
//          Efficiency = 75.0<percentage>
//          BoilLength = 60.0<minute>
//          MashLength = 60.0<minute>
//          Volume = 21.0<L>
//          Style = ""
//          EstimatedOriginalGravity = 0.0<sg> }

    let grain = ObservableCollection<GrainViewModel>()

    let RefreshParts = 
        this.GetModel() |> ignore

    let addMaltCommand = 
        this.Factory.CommandSync(fun param -> 
            let g = { Name = "Maris Otter"
                      Weight = 0.001<kg>
                      Potential = 37.0<pgpkg>
                      Colour = 4.0<EBC> }
            //recipe <- AddGrain recipe g
            this.Grain.Add(GrainViewModel(g))
            RefreshParts)
                                        

    member x.AddMaltCommand = addMaltCommand
    member x.Grain:ObservableCollection<GrainViewModel> = grain

    member private x.UpdateModel recipe=
        grain 
        |> Seq.map (fun g -> g.GetModel()) |> Seq.toList
        |> UpdateGrain recipe
        |> EstimateOriginalGravity
        