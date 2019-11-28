# Copyright © 2019 eMedia Intellect.

# This file is part of eMI Spanish Verb Conjugator.

# eMI Spanish Verb Conjugator is free software: you can redistribute it and/or modify
# it under the terms of the GNU General Public License as published by
# the Free Software Foundation, either version 3 of the License, or
# (at your option) any later version.

# eMI Spanish Verb Conjugator is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
# GNU General Public License for more details.

# You should have received a copy of the GNU General Public License
# along with eMI Spanish Verb Conjugator. If not, see http://www.gnu.org/licenses/.

#####################################################
# Solution cleaning for eMI Spanish Verb Conjugator #
#####################################################

Write-Host '┌───────────────────────────────────────────────────┐'
Write-Host '│ Solution cleaning for eMI Spanish Verb Conjugator │'
Write-Host '└───────────────────────────────────────────────────┘'
Write-Host

Write-Host 'Removing the Visual Studio solution user options directory.'
Write-Host

Remove-Item -ErrorAction 'Ignore' -Force -Path '.\.vs' -Recurse

Write-Host 'Removing the builds.'
Write-Host

Remove-Item -ErrorAction 'Ignore' -Force -Path '.\ConsoleApplication\Build' -Recurse
Remove-Item -ErrorAction 'Ignore' -Force -Path '.\Library\Build' -Recurse
Remove-Item -ErrorAction 'Ignore' -Force -Path '.\WindowsApplication\Build' -Recurse