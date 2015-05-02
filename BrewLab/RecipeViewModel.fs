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
    inherit LabViewModel<_recipe<kg, L, degC>>(recipe)
    let grain = ObservableCollection<GrainViewModel>()
    let hopAdditions = ObservableCollection<HopViewModel>()
    let refreshCommand = this.Factory.CommandSync(fun param -> this.RefreshParts)
    let handleRefresh event = this.RefreshParts
    
    let addIngredient = function 
        | Hop h -> this.HopAdditions.Add(new HopViewModel(h))
        | Grain g -> this.Grain.Add(new GrainViewModel(g))
        | _ -> () //TODO - handle adjuncts
    
    let addMaltCommand = 
        this.Factory.CommandSync(fun p -> 
            addIngredient <| Grain { Grain = this.Grains.[0];  Weight = 0.0<kg> } //Default to first in list - Could be empty instead?
            this.RefreshParts)
    
    let removeMaltCommand = 
        this.Factory.CommandSync(fun param -> 
            let g = this.Grain.Item(this.Grain.Count - 1)
            this.Grain.Remove(g) |> ignore
            (g :> IDisposable).Dispose())
    
    let addHopCommand = 
        this.Factory.CommandSync(fun p -> 
            addIngredient <| Hop { Hop = this.Hops.[0]
                                   Weight = 0.0<g>
                                   Time = 0.0<minute>
                                   Type = HopType.Leaf } //Default to first in list - Could be empty instead?
            this.RefreshParts)
    
    let removeHopCommand = 
        this.Factory.CommandSync(fun param -> 
            let hop = this.HopAdditions.Item(this.HopAdditions.Count - 1)
            this.HopAdditions.Remove(hop) |> ignore
            (hop :> IDisposable).Dispose())
    
    //this.Grain.RemoveAt(this.Grain.Count-1)) //Default to first in list - Could be empty instead?
    //Temp fixed list of grain
    let grains = 
        [ { Name = "Maris Otter"
            Potential = 37.0<pgpkg>
            Colour = 4.0<EBC> }
          { Name = "Cara Amber"
            Potential = 35.0<pgpkg>
            Colour = 20.0<EBC> }
          { Name = "Cara Pils"
            Potential = 32.0<pgpkg>
            Colour = 10.0<EBC> } ]
    
    let hops = 
        [ { Name = "East Kent Goldings"
            Alpha = 7.9<percentage> }
          { Name = "Northen Brewer"
            Alpha = 11.0<percentage> } ]
    
    do base.BindEvent(Events.RecipeEvent.Instance.Event, (fun o -> o :> IObservable<_>), handleRefresh)
    member x.Grains : grain<kg> list = grains //TODO - Move to a standing data service
    member x.Hops : hop<g> list = hops //TODO - Move to a standing data service
    member x.AddMaltCommand = addMaltCommand
    member x.AddHopCommand = addHopCommand
    member x.RemoveMaltCommand = removeMaltCommand
    member x.RemoveHopCommand = removeHopCommand
    member x.Grain : ObservableCollection<GrainViewModel> = grain
    member x.HopAdditions : ObservableCollection<HopViewModel> = hopAdditions
    member private x.RefreshParts = this.UpdateModelSnapshot()
    override x.UpdateModel recipe = 
        grain
        |> Seq.map (fun g -> g.GetModel())
        |> Seq.toList
        |> UpdateGrain recipe
        |> EstimateOriginalGravity