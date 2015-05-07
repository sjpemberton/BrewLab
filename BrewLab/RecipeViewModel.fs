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
    inherit LabViewModel<_recipe<kg, L, degC>>(recipe, Events.LabEvent.GravityChange)
    let ibu = this.Factory.Backing(<@ this.Bitterness @>, recipe.Bitterness)
    let volume = this.Factory.Backing(<@ this.Volume @>, recipe.Volume)
    let efficiency = this.Factory.Backing(<@ this.Efficiency @>, recipe.Efficiency)
    let estimatedGravity = this.Factory.Backing(<@ this.Gravity @>, recipe.EstimatedOriginalGravity)
    let grain = ObservableCollection<GrainViewModel>()
    let hopAdditions = ObservableCollection<HopViewModel>()
    //let handleRefresh event = this.RefreshParts
    
    let addIngredient = 
        function 
        | Hop h -> this.HopAdditions.Add(new HopViewModel(h, estimatedGravity.Value, volume.Value))
        | Grain g -> this.Grain.Add(new GrainViewModel(g))
        | _ -> () //TODO - handle adjuncts
    
    let sumIBUs =
        hopAdditions
        |> Seq.sumBy (fun h -> h.IBU)

    let handleIngredientUpdate = function
        Events.FermentableChange -> this.Gravity <- grain
                                                    |> Seq.map (fun g -> g.GetModel())
                                                    |> Seq.toList
                                                    |> CalculateGravity this.Volume this.Efficiency
                                    hopAdditions |> Seq.iter (fun h -> h.UpdateRecipeDetails(this.Gravity, this.Volume))
        | Events.HopChange -> this.Bitterness <- sumIBUs
        | _ -> ()

    let addMaltCommand = 
        this.Factory.CommandSync(fun p -> 
            addIngredient <| Grain { Grain = this.Grains.[0]; Weight = 0.0<kg> }
            this.RefreshParts)

    let removeMaltCommand = 
        this.Factory.CommandSyncParam(fun grainVm -> 
            this.Grain.Remove(grainVm) |> ignore
            this.RefreshParts
            handleIngredientUpdate Events.FermentableChange
            (grainVm :> IDisposable).Dispose())
    
    let addHopCommand = 
        this.Factory.CommandSync(fun p -> 
            addIngredient <| Hop { Hop = this.Hops.[0]; Weight = 0.0<g>; Time = 0.0<minute>; Type = HopType.Leaf }
            this.RefreshParts)

    let removeHopCommand = 
        this.Factory.CommandSyncParam(fun hopVm -> 
            this.HopAdditions.Remove(hopVm) |> ignore
            this.RefreshParts
            handleIngredientUpdate Events.HopChange
            (hopVm :> IDisposable).Dispose())
    
    do 
        base.BindEvent(Events.RecipeEvent.Instance.Event, 
                       (Observable.filter (function 
                            | Events.FermentableChange | Events.HopChange -> true
                            | _ -> false)), handleIngredientUpdate)
    
    member x.Grain : ObservableCollection<GrainViewModel> = grain
    member x.HopAdditions : ObservableCollection<HopViewModel> = hopAdditions
    
    member x.Gravity with get () = estimatedGravity.Value and set (v) = estimatedGravity.Value <- v
    member x.Volume with get () = volume.Value and set (v) = volume.Value <- v
    member x.Bitterness with get () = ibu.Value and set (v) = ibu.Value <- v
    member x.Efficiency with get () = efficiency.Value and set (v) = efficiency.Value <- v

    member x.Grains : grain<kg> list = DataService.grains 
    member x.Hops : hop<g> list = DataService.hops
    member x.HopTypes = DataService.hopTypes

    member x.AddMaltCommand = addMaltCommand
    member x.AddHopCommand = addHopCommand
    member x.RemoveMaltCommand = removeMaltCommand
    member x.RemoveHopCommand = removeHopCommand

    member private x.RefreshParts = this.UpdateModelSnapshot()

    override x.UpdateModel recipe = 
        grain
        |> Seq.map (fun g -> g.GetModel())
        |> Seq.toList
        |> UpdateGrain recipe
        |> EstimateOriginalGravity