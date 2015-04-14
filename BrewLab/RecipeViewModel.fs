﻿namespace ViewModels

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

    let addMaltCommand malt = 
        this.Factory.CommandSyncParam(fun param -> 
            this.Grain.Add(GrainViewModel(malt))
            this.RefreshParts)

    //Temp fixed list of grain
    let grains = [{ Name = "Maris Otter"
                    Weight = 0.0<kg>
                    Potential = 37.0<pgpkg>
                    Colour = 4.0<EBC> };
                  { Name = "Cara Amber"
                    Weight = 0.0<kg>
                    Potential = 35.0<pgpkg>
                    Colour = 20.0<EBC> };
                  { Name = "Cara Pils"
                    Weight = 0.0<kg>
                    Potential = 32.0<pgpkg>
                    Colour = 10.0<EBC> }]
                                        
    member x.Grains = grains
    member x.AddMaltCommand = addMaltCommand
    member x.Grain:ObservableCollection<GrainViewModel> = grain

    member private x.RefreshParts = 
            this.GetModel() |> ignore

    override x.UpdateModel recipe=
        grain 
        |> Seq.map (fun g -> g.GetModel()) |> Seq.toList
        |> UpdateGrain recipe
        |> EstimateOriginalGravity
        
    