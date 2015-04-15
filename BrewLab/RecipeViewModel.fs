namespace ViewModels

open System.Collections.ObjectModel
open System.Windows.Data
open FSharp.ViewModule
open FsXaml
open Units
open Models.Recipe
open Models
open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols
open System.ComponentModel
open System

type RecipeViewModel(recipe) as this = 
    inherit LabViewModel<_recipe<kg,L,degC>>(recipe)

    let grain = ObservableCollection<GrainViewModel>()

    let addMaltCommand = 
        this.Factory.CommandSync(fun param ->
            this.Grain.Add(GrainViewModel({Grain = this.Grains.[0]; Weight = 0.0<kg>})) //Default to first in list - Could be empty instead?
            this.RefreshParts)

//    let switchGrainCommand = 
//        this.Factory.CommandSyncParam(fun (param:obj) ->
//            match param with 
//            | :? System.Windows.Controls.SelectionChangedEventArgs as e->
//                this.Grain.Add(GrainViewModel(t.Item1 :?> grain<kg>)) //Need to stop this being a tuple
//                this.RefreshParts
//            | _ -> ignore())
          

    //Temp fixed list of grain
    let grains = [{ Name = "Maris Otter"
                    Potential = 37.0<pgpkg>
                    Colour = 4.0<EBC> };
                  { Name = "Cara Amber"
                    Potential = 35.0<pgpkg>
                    Colour = 20.0<EBC> };
                  { Name = "Cara Pils"
                    Potential = 32.0<pgpkg>
                    Colour = 10.0<EBC> }]
                                        
    member x.Grains: grain<kg> list = grains
    member x.AddMaltCommand = addMaltCommand
    member x.SwitchGrainCommand = switchGrainCommand
    member x.Grain:ObservableCollection<GrainViewModel> = grain

    member private x.RefreshParts = 
            this.GetModel() |> ignore

    override x.UpdateModel recipe=
        grain 
        |> Seq.map (fun g -> g.GetModel()) |> Seq.toList
        |> UpdateGrain recipe
        |> EstimateOriginalGravity
        
    