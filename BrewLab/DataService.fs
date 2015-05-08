module DataService

open Models
open Units

let grains = 
    [ { Name = "Maris Otter"
        Potential = 308.765<pgpkg>
        Colour = 4.0<EBC> }
      { Name = "Cara Amber"
        Potential = 292.075<pgpkg>
        Colour = 20.0<EBC> }
      { Name = "Cara Pils"
        Potential = 267.04<pgpkg>
        Colour = 10.0<EBC> } ]
    
let hops = 
    [ { Name = "East Kent Goldings"
        Alpha = 7.9<percentage> }
      { Name = "Northen Brewer"
        Alpha = 11.0<percentage> } ]
    
let hopTypes = [ Leaf; Pellet; Extract ]