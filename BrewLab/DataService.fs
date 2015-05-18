module DataService

open Models
open Units

let grains = 
    [ Grain ( Potential 308.765<pgpkg>,
        4.0<EBC>,
        "Maris Otter")
      Grain ( Potential 292.075<pgpkg>,
        20.0<EBC>, 
        "Cara Amber")
      Grain ( Potential 267.04<pgpkg>,
        10.0<EBC>,
        "Cara Pils")]
    
let hops = 
    [ Hop (Alpha 7.9<percentage>, "East Kent Goldings")
      Hop (Alpha  11.0<percentage>, "Northen Brewer") ]
    
let hopTypes = [ Leaf; Pellet; Extract ]