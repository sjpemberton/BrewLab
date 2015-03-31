namespace ViewModels

open System.Collections.ObjectModel
open System.Windows.Data
open FSharp.ViewModule
open Units
open Models.Recipe
open Models
open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols
open System.ComponentModel

type RecipeViewModel() as this = 
    inherit ViewModelBase()
    let mutable recipe = 
        { Name = ""
          Grain = List.Empty
          Hops = List.Empty
          Adjuncts = List.Empty
          Yeast = None
          Efficiency = 75.0<percentage>
          BoilLength = 60.0<minute>
          MashLength = 60.0<minute>
          Volume = 21.0<L>
          Style = ""
          EstimatedOriginalGravity = 0.0<sg> }

    //let grain = ObservableCollection<GrainViewModel>()

    let mutable test = 
        [| 
            GrainViewModel({Name = "Maris Otter"; Weight = 0.001<kg>; Potential = 37.0<pgpkg>; Colour = 4.0<EBC>} )
            GrainViewModel({Name = "Maris Otter2"; Weight = 0.001<kg>; Potential = 37.0<pgpkg>; Colour = 4.0<EBC>})
        |]

    let addMaltCommand = 
        this.Factory.CommandSync
            (fun param -> 
                 recipe <- AddGrain recipe {Name = "Maris Otter"; Weight = 0.001<kg>; Potential = 37.0<pgpkg>; Colour = 4.0<EBC>}
                 this.Grain.Refresh()
                 this.RaisePropertyChanged "Grain")

    
    member x.AddMaltCommand = addMaltCommand
    member x.Grain with get():ICollectionView = CollectionViewSource.GetDefaultView( recipe.Grain |> List.map(fun g -> GrainViewModel(g)))