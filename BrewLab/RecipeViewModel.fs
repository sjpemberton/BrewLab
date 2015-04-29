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

    let refreshCommand =  this.Factory.CommandSync(fun param ->
        this.RefreshParts)

    let handleRefresh vmEvent =
        this.RefreshParts

    let addMaltCommand = 
        this.Factory.CommandSync(fun param ->
            let gvm = new GrainViewModel({Grain = this.Grains.[0]; Weight = 0.0<kg>})
            this.Grain.Add(gvm) //Default to first in list - Could be empty instead?
            this.RefreshParts)

    let removeMaltCommand =
        this.Factory.CommandSync(fun param ->
            let g = this.Grain.Item(this.Grain.Count-1) 
            this.Grain.Remove(g) |> ignore
            (g :> IDisposable).Dispose())
            //this.Grain.RemoveAt(this.Grain.Count-1)) //Default to first in list - Could be empty instead?

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

//    do
//        this.Disposable <- Some (Observable.subscribe this.processUpdate this.Event.Event)  
//                                        
    member x.Grains: grain<kg> list = grains
    member x.AddMaltCommand = addMaltCommand
    member x.RemoveMaltCommand = removeMaltCommand
    member x.Grain:ObservableCollection<GrainViewModel> = grain
    

    member private x.RefreshParts = 
            this.UpdateModelSnapshot()

    override x.processUpdate e = 
        handleRefresh e

    override x.UpdateModel recipe =
        grain 
        |> Seq.map (fun g -> g.GetModel()) |> Seq.toList
        |> UpdateGrain recipe
        |> EstimateOriginalGravity
        
    