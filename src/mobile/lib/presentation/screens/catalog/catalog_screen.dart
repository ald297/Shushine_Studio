import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../data/models/category_dto.dart';
import '../../../data/models/service_dto.dart';
import '../../blocs/catalog/catalog_bloc.dart';
import '../../blocs/catalog/catalog_event.dart';
import '../../blocs/catalog/catalog_state.dart';
import 'service_detail_screen.dart';

class CatalogScreen extends StatefulWidget {
  const CatalogScreen({super.key});

  @override
  State<CatalogScreen> createState() => _CatalogScreenState();
}

class _CatalogScreenState extends State<CatalogScreen> {
  final _searchController = TextEditingController();

  @override
  void initState() {
    super.initState();
    context.read<CatalogBloc>().add(CatalogFetchRequested());
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    const primaryColor = Color(0xFFC5A059);
    const backgroundColor = Color(0xFF121214);
    const surfaceColor = Color(0xFF1E1E24);
    const cardColor = Color(0xFF26262E);
    const textColor = Color(0xFFF5F5F7);
    const subtitleColor = Color(0xFFA0A0AB);

    return Scaffold(
      backgroundColor: backgroundColor,
      appBar: AppBar(
        backgroundColor: Colors.transparent,
        elevation: 0,
        title: const Text(
          'Catálogo de Belleza',
          style: TextStyle(color: textColor, fontWeight: FontWeight.bold),
        ),
        centerTitle: true,
      ),
      body: BlocBuilder<CatalogBloc, CatalogState>(
        builder: (context, state) {
          return Column(
            children: [
              // Barra de Búsqueda
              Padding(
                padding: const EdgeInsets.symmetric(horizontal: 16.0, vertical: 8.0),
                child: TextField(
                  controller: _searchController,
                  style: const TextStyle(color: textColor),
                  decoration: InputDecoration(
                    hintText: 'Buscar corte, balayage, manicura...',
                    hintStyle: const TextStyle(color: subtitleColor, fontSize: 14),
                    prefixIcon: const Icon(Icons.search_rounded, color: primaryColor),
                    suffixIcon: _searchController.text.isNotEmpty
                        ? IconButton(
                            icon: const Icon(Icons.clear, color: subtitleColor, size: 20),
                            onPressed: () {
                              _searchController.clear();
                              context.read<CatalogBloc>().add(const CatalogSearchChanged(''));
                            },
                          )
                        : null,
                    filled: true,
                    fillColor: surfaceColor,
                    contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
                    border: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(14),
                      borderSide: BorderSide.none,
                    ),
                    focusedBorder: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(14),
                      borderSide: const BorderSide(color: primaryColor, width: 1.5),
                    ),
                  ),
                  onChanged: (val) {
                    context.read<CatalogBloc>().add(CatalogSearchChanged(val));
                  },
                ),
              ),

              // Carrusel Horizontal de Categorías
              if (state is CatalogLoaded && state.categories.isNotEmpty)
                _buildCategoryChips(
                  state.categories,
                  state.selectedCategoryId,
                  primaryColor,
                  surfaceColor,
                  textColor,
                  subtitleColor,
                ),

              const SizedBox(height: 8),

              // Lista de Servicios
              Expanded(
                child: RefreshIndicator(
                  color: primaryColor,
                  onRefresh: () async {
                    context.read<CatalogBloc>().add(CatalogFetchRequested());
                  },
                  child: _buildContent(state, primaryColor, surfaceColor, cardColor, textColor, subtitleColor),
                ),
              ),
            ],
          );
        },
      ),
    );
  }

  Widget _buildCategoryChips(
    List<CategoryDto> categories,
    int? selectedId,
    Color primaryColor,
    Color surfaceColor,
    Color textColor,
    Color subtitleColor,
  ) {
    return SizedBox(
      height: 44,
      child: ListView.separated(
        padding: const EdgeInsets.symmetric(horizontal: 16),
        scrollDirection: Axis.horizontal,
        itemCount: categories.length + 1,
        separatorBuilder: (_, __) => const SizedBox(width: 8),
        itemBuilder: (context, index) {
          final isAll = index == 0;
          final isSelected = isAll ? selectedId == null : selectedId == categories[index - 1].id;
          final label = isAll ? 'Todos' : categories[index - 1].nombre;

          return ChoiceChip(
            label: Text(
              label,
              style: TextStyle(
                color: isSelected ? Colors.black : textColor,
                fontSize: 13,
                fontWeight: isSelected ? FontWeight.bold : FontWeight.w500,
              ),
            ),
            selected: isSelected,
            selectedColor: primaryColor,
            backgroundColor: surfaceColor,
            showCheckmark: false,
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(20),
              side: BorderSide(
                color: isSelected ? primaryColor : const Color(0xFF2E2E38),
                width: 1,
              ),
            ),
            onSelected: (_) {
              final catId = isAll ? null : categories[index - 1].id;
              context.read<CatalogBloc>().add(CatalogCategorySelected(catId));
            },
          );
        },
      ),
    );
  }

  Widget _buildContent(
    CatalogState state,
    Color primaryColor,
    Color surfaceColor,
    Color cardColor,
    Color textColor,
    Color subtitleColor,
  ) {
    if (state is CatalogLoading) {
      return const Center(
        child: CircularProgressIndicator(color: Color(0xFFC5A059)),
      );
    }

    if (state is CatalogError) {
      return Center(
        child: Padding(
          padding: const EdgeInsets.all(24.0),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              const Icon(Icons.wifi_off_rounded, color: Color(0xFFEF5350), size: 48),
              const SizedBox(height: 12),
              Text(
                state.message,
                textAlign: TextAlign.center,
                style: const TextStyle(color: Color(0xFFA0A0AB), fontSize: 14),
              ),
              const SizedBox(height: 16),
              ElevatedButton.icon(
                onPressed: () {
                  context.read<CatalogBloc>().add(CatalogFetchRequested());
                },
                icon: const Icon(Icons.refresh, size: 18),
                label: const Text('Reintentar'),
                style: ElevatedButton.styleFrom(
                  backgroundColor: primaryColor,
                  foregroundColor: Colors.black,
                ),
              ),
            ],
          ),
        ),
      );
    }

    if (state is CatalogLoaded) {
      if (state.services.isEmpty) {
        return const Center(
          child: Text(
            'No se encontraron servicios disponibles en esta categoría.',
            style: TextStyle(color: Color(0xFFA0A0AB), fontSize: 14),
          ),
        );
      }

      return ListView.builder(
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
        itemCount: state.services.length,
        itemBuilder: (context, index) {
          final service = state.services[index];
          return _serviceCard(service, primaryColor, surfaceColor, textColor, subtitleColor);
        },
      );
    }

    return const SizedBox.shrink();
  }

  Widget _serviceCard(
    ServiceDto service,
    Color primaryColor,
    Color surfaceColor,
    Color textColor,
    Color subtitleColor,
  ) {
    return Container(
      margin: const EdgeInsets.only(bottom: 14),
      decoration: BoxDecoration(
        color: surfaceColor,
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: const Color(0x33C5A059), width: 0.8),
      ),
      child: Material(
        color: Colors.transparent,
        child: InkWell(
          borderRadius: BorderRadius.circular(16),
          onTap: () {
            Navigator.of(context).push(
              MaterialPageRoute(
                builder: (_) => ServiceDetailScreen(service: service),
              ),
            );
          },
          child: Padding(
            padding: const EdgeInsets.all(14.0),
            child: Row(
              children: [
                // Thumbnail
                ClipRRect(
                  borderRadius: BorderRadius.circular(12),
                  child: SizedBox(
                    width: 76,
                    height: 76,
                    child: service.imagenUrl != null && service.imagenUrl!.isNotEmpty
                        ? CachedNetworkImage(
                            imageUrl: service.imagenUrl!,
                            fit: BoxFit.cover,
                            placeholder: (_, __) => Container(color: const Color(0xFF26262E)),
                            errorWidget: (_, __, ___) => Container(
                              color: const Color(0xFF26262E),
                              child: const Icon(Icons.spa, color: Color(0xFFC5A059)),
                            ),
                          )
                        : Container(
                            color: const Color(0xFF26262E),
                            child: const Icon(Icons.spa, color: Color(0xFFC5A059)),
                          ),
                  ),
                ),
                const SizedBox(width: 14),

                // Details
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      if (service.categoriaNombre != null)
                        Text(
                          service.categoriaNombre!.toUpperCase(),
                          style: TextStyle(
                            color: primaryColor,
                            fontSize: 10,
                            fontWeight: FontWeight.bold,
                            letterSpacing: 0.8,
                          ),
                        ),
                      const SizedBox(height: 2),
                      Text(
                        service.nombre,
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                        style: TextStyle(
                          color: textColor,
                          fontSize: 16,
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                      const SizedBox(height: 4),
                      Row(
                        children: [
                          Icon(Icons.schedule, size: 14, color: subtitleColor),
                          const SizedBox(width: 4),
                          Text(
                            service.duracionFormateada,
                            style: TextStyle(color: subtitleColor, fontSize: 12),
                          ),
                        ],
                      ),
                      const SizedBox(height: 4),
                      Text(
                        service.precioFormateado,
                        style: TextStyle(
                          color: primaryColor,
                          fontSize: 15,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ],
                  ),
                ),

                const Icon(Icons.arrow_forward_ios_rounded, color: Color(0xFFA0A0AB), size: 16),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
