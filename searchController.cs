<!-- Ajoutez cette ligne dans la section head de votre HTML -->
<script src="https://code.jquery.com/jquery-3.6.4.min.js"></script>

<!-- Votre formulaire de recherche -->
<form class="d-none d-md-inline-block form-inline ms-auto me-0 me-md-3 my-2 my-md-0" id="searchForm">
    <div class="input-group">
        <input class="form-control" type="text" id="searchInput" placeholder="Search for..." aria-label="Search for..." aria-describedby="btnNavbarSearch" />
        <button class="btn btn-primary" id="btnNavbarSearch" type="button"><i class="fas fa-search"></i></button>
    </div>
</form>

<!-- Div pour afficher les résultats -->
<div id="searchResults"></div>

<!-- Script JavaScript -->
<script>
    $(document).ready(function () {
        // Lorsque le bouton de recherche est cliqué
        $("#btnNavbarSearch").click(function () {
            // Récupérer la valeur de l'entrée de recherche
            var searchTerm = $("#searchInput").val();

            // Effectuer une requête AJAX pour récupérer les résultats
            $.ajax({
                url: "votre_script_de_recherche.php", // Remplacez ceci par le chemin de votre script de recherche serveur
                method: "POST",
                data: { searchTerm: searchTerm },
                success: function (data) {
                    // Afficher les résultats dans la div des résultats
                    $("#searchResults").html(data);
                },
                error: function (error) {
                    console.error("Erreur lors de la requête AJAX", error);
                }
            });
        });
    });
</script>
