Change history for [SMARTMARKET][1]
===================================


Version 7.0 - *Jun 20, 2019*
----------------------------

#### 6/20/19
  - Release 7.0

#### 1/11/19
  - Add `test_smartmarket.m` test suite.
  - Add Travis-CI integration.


Version 6.0 - *Dec 16, 2016*
----------------------------


Version 5.1 - *Mar 20, 2015*
----------------------------


Version 5.0 - *Dec 17, 2014*
----------------------------


Version 5.0b1 - *Jul 1, 2014*
-----------------------------

#### 3/27/12
  - Fixed bug in `smartmkt.m` where the `gencost` returned in the
    results struct was not the original `gencost`, but rather the
    one formed from the offers and bids. That one is now returned
    in a `genoffer` field, with the original being returned in `gencost`.

#### 3/1/12
  - Fixed bug in `smartmkt.m` that caused an when including reactive
    power costs.


Version 4.1 - *Dec 14, 2011*
---------------------------

#### 3/23/11
  - Fixed bug in `smartmkt.m` that caused error when using offers and
    bids with different numbers of blocks.


Version 4.0 - *Feb 7, 2011*
---------------------------

#### 4/19/10
  - Changed licensing to GNU General Public license. See `LICENSE` and
    `COPYING` files for details.

#### 3/23/09
  - Fixed bug in `smartmkt()` that caused it to die for cases with
    non-consecutive bus numbers. Introduced by changes made to
    `runmarket()` in 4.0b1.


Version 4.0b2 - *Mar 19, 2010*
------------------------------

#### 2/11/10
  - Fixed bug in design of auction code. Prices are now scaled instead
    of shifted when modified according specified pricing rule
    (e.g. LAO, FRO, LAB, FRB, split-the-difference, etc.). Auctions
    with both real and reactive offers/bids must be type 0 or 5, type
    1 = LAO is no longer allowed.


Version 4.0b1 - *Dec 24, 2009*
------------------------------

#### 12/24/09
  - Released version 4.0b1.

#### 12/11/09
  - Updated `runmarket`, `smartmkt`, `printmkt` to pass full OPF results
    struct through to output.

#### 12/8/09
  - Miscellaneous cleanup based on mlint suggestions.

#### 11/4/09
  - Removed unnecessary `return` statement at end of all M-files. If
    anything it should be an `end` statement, but even that is
    optional, so we just let functions get terminated by the
    end-of-file or another function declaration.

#### 4/14/09
  - Deprecate use of `areas` data matrix. Remove it everywhere
    possible without breaking backward compatibility with version 1
    case files, which required it.


Version 3.2 - *Sep 21, 2007*
----------------------------

#### 5/15/07
  - Fixed a display bug where it would show the offer/bid price rather than
    the price of the cleared offer/bid for generators with zero output.


Version 3.1b2 - *Sep 15, 2006*
------------------------------

#### 9/15/06
  - Released version 3.1b2.


Version 3.1b1 - *Aug 1, 2006*
-----------------------------

#### 8/1/06
  - Released version 3.1b1.

#### 7/10/06
  - Corrected costs of dispatchable gens in `t_auction_case.m` so that the
    cost is 0 at 0 output and negative at Pmin.

#### 4/17/06
  - Fixed some bugs in `off2case.m` and added additional tests related to
    shutting down loads with zero qty bids. It now properly shuts down
    any load with a zero qty P bid, and loads with zero qty Q bids if and
    only if the load has a non-unity power factor.

#### 10/26/05
  - Fixed bug in `off2case.m` where P-only load was turned off by zero qty Q
    bid. Added corresponding test.
  - Fix in `off2case.m` for div by zero when Q portion of gencost is not
    specified and we have zero Q bid qty.

#### 10/17/05
  - Rewrite of much of the code to handle reactive power. Added `runmarket.m`,
    `pricelimits.m`. Made `runmkt` a wrapper around new `runmarket`.

#### 8/11/05
  - Fixed a bug in `smartmkt.m` which caused incorrect prices to be printed
    in the market summary for single block offers.

#### 4/25/05
  - Fixed a bug in `auction.m` where prices for rejected bids were computed
    erroneously based on a unity power factor instead of the one defined
    by PMIN and the Q limits.

#### 4/14/05
  - Fixed a bug in `auction.m` where a withheld offer could end up with
    a tiny bit being accepted, which messed up the prices.

#### 4/6/05
  - Shifted the total cost function generated for bids to intersect the
    origin instead of (Pmin, 0) in `off2case.m`. This results in objective
    function values that are consistent across different sets of bids.


Version 3.0.0 - *Feb 14, 2005*
------------------------------

#### 1/24/05
  - Switched to using `isload()` to check for dispatchable load.
  - Modified `off2case.m` to keep Q limits consistent with constant power
    factor for dispatchable loads.

#### 11/18/04
  - Fixed bug in `off2case.m` for inputs containing zero quantity bids.

#### 9/28/04
  - Modified to `auction.m` to clip cleared offer prices (but not bid prices)
    to `max_p`. Other changes to make it more robust for cases with
    very large bid/offer gap.
  
  - In `smartmkt.m` added conditionals to skip over code related to import
    gens (including offer and `max_p` adjustments) when there are no import
    generators. Fixed missing return args bug when OPF did not converge.


Version 3.0b3 - *Sep 20, 2004*
------------------------------

#### 9/15/04
  - Fixed bug in `auction.m` where locational prices were
    wiped out when generators had only a single block.
  - Fixed small bug in `smartmkt.m` related to single block
    generators, and worked around another related to handling
    of reserve generators when gencost and genoffer are
    not the same dimension.
    

Version 3.0b2 - *Sep 7, 2004*
-----------------------------

#### 7/2/04
  - Fixed `smartmarket.m` to print correct prices even for decommitted
    gens.
  - Fixed `auction.m` so that gens at Pmin are not able to set the price
    and each gen still gets a uniform price even if price is clipped by
    the offer/bid of a single block. Added lots of tests.

#### 5/30/03
  - Added `auction.m`, removed everything to do with continuous
    offer/bid markets.

#### 2/14/01
  - Corrected a comment.

#### 4/3/00
  - Modified `smartmkt.m` so that reserve generators come in, not
    at the reservation price, but at $5 above the highest
    offer price (excluding offers above reservation price).

#### 3/14/00
  - Made changes to `off2case.m` to fix problems with negative
    generation.

#### 11/9/99 - version 2.5b3


---

[1]: https://github.com/MATPOWER/matpower-extras/blob/master/smartmarket/SM_CHANGES.md
