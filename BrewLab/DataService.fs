module DataService

open Models
open Units

let fermentables = 
    [ Grain ( 308.765<pgpkg>,
        4.0<EBC>,
        "Maris Otter")
      Grain ( 292.075<pgpkg>,
        20.0<EBC>, 
        "Cara Amber")
      Grain ( 267.04<pgpkg>,
        10.0<EBC>,
        "Cara Pils")
      Adjunct(10.0<pgpkg>,
        "Orange Zest")]
    
let hops = 
    [ Hop (Alpha 7.9<percentage>, "East Kent Goldings")
      Hop (Alpha  11.0<percentage>, "Northen Brewer") ]
    
let hopTypes = [ Leaf; Pellet; Extract ]