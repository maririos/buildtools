#!/usr/bin/env bash

working_tree_root="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
echo $working_tree_root/corerun $working_tree_root/MSBuild.exe $*
$working_tree_root/corerun $working_tree_root/MSBuild.exe $*
exit $ 