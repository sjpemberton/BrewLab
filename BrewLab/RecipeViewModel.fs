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

    let colour = this.Factory.Backing(<@ this.Colour @>, recipe.Colour)
    let ibu = this.Factory.Backing(<@ this.Bitterness @>, recipe.Bitterness)
    let volume = this.Factory.Backing(<@ this.Volume @>, recipe.Volume)
    let efficiency = this.Factory.Backing(<@ this.Efficiency @>, recipe.Efficiency)
    let estimatedGravity = this.Factory.Backing(<@ this.Gravity @>, recipe.EstimatedOriginalGravity)
    let grain = ObservableCollection<GrainViewModel>()
    let hopAdditions = ObservableCollection<HopViewModel>()
    
    let addIngredient = 
        function 
        | Hop h -> this.HopAdditions.Add(new HopViewModel(h, estimatedGravity.Value, volume.Value))
        | Fermentable (Grain g) -> this.Grain.Add(new GrainViewModel(g))
        | _ -> () //TODO - handle adjuncts
    
    let sumIBUs =
        hopAdditions
        |> Seq.sumBy (fun h -> h.IBU)

    let handleIngredientUpdate = function
        Events.FermentableChange -> 
            this.Gravity <- grain
                            |> Seq.map (fun g -> Grain <| g.GetUpdatedModel())
                            |> Seq.toList
                            |> CalculateGravity this.Volume this.Efficiency
            hopAdditions |> Seq.iter (fun h -> h.UpdateRecipeDetails(this.Gravity, this.Volume))
        | Events.HopChange -> 
            ibu.Value <- sumIBUs
        | _ -> ()

    let addMaltCommand = 
        this.Factory.CommandSync(fun p -> 
            addIngredient <| Fermentable (Grain { Grain = this.Grains.[0]; Weight = 0.0<kg> }))

    let removeMaltCommand = 
        this.Factory.CommandSyncParam(fun grainVm -> 
            this.Grain.Remove(grainVm) |> ignore
            handleIngredientUpdate Events.FermentableChange
            (grainVm :> IDisposable).Dispose())
    
    let addHopCommand = 
        this.Factory.CommandSync(fun p -> 
            addIngredient <| Hop { Hop = this.Hops.[0]; Weight = 0.0<g>; Time = 0.0<minute>; Type = HopType.Leaf })

    let removeHopCommand = 
        this.Factory.CommandSyncParam(fun hopVm -> 
            this.HopAdditions.Remove(hopVm) |> ignore
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
    member x.Bitterness = ibu.Value
    member x.Efficiency with get () = efficiency.Value and set (v) = efficiency.Value <- v
    member x.Colour = colour.Value

    member x.Grains : grain<kg> list = DataService.grains 
    member x.Hops : hop<g> list = DataService.hops
    member x.HopTypes = DataService.hopTypes

    member x.AddMaltCommand = addMaltCommand
    member x.AddHopCommand = addHopCommand
    member x.RemoveMaltCommand = removeMaltCommand
    member x.RemoveHopCommand = removeHopCommand

    override x.GetUpdatedModel() = 
        grain
        |> Seq.map (fun g -> Grain <| g.GetUpdatedModel())
        |> Seq.toList
        |> UpdateFermentables recipe
        |> EstimateOriginalGravity