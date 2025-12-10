using System;
using System.Collections.Generic;
using Episodes.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Episodes.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Episode> Episodes { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Season> Seasons { get; set; }

    public virtual DbSet<Show> Shows { get; set; }

    public virtual DbSet<TvDataProvider> TvDataProviders { get; set; }

    public virtual DbSet<TvNetwork> TvNetworks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserShow> UserShows { get; set; }

    public virtual DbSet<WatchedEpisode> WatchedEpisodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Episode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("episodes_pkey");

            entity.ToTable("episodes", "episodes");

            entity.HasIndex(e => new { e.SeasonId, e.EpisodeNumber }, "episodes_season_id_episode_number_key").IsUnique();

            entity.HasIndex(e => e.SeasonId, "episodes_season_id_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AirDate).HasColumnName("air_date");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.DataProviderId).HasColumnName("data_provider_id");
            entity.Property(e => e.EpisodeNumber).HasColumnName("episode_number");
            entity.Property(e => e.ExternalId).HasColumnName("external_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Overview).HasColumnName("overview");
            entity.Property(e => e.SeasonId).HasColumnName("season_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.DataProvider).WithMany(p => p.Episodes)
                .HasForeignKey(d => d.DataProviderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("episodes_data_provider_id_fkey");

            entity.HasOne(d => d.Season).WithMany(p => p.Episodes)
                .HasForeignKey(d => d.SeasonId)
                .HasConstraintName("episodes_season_id_fkey");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("genres_pkey");

            entity.ToTable("genres", "episodes");

            entity.HasIndex(e => e.Name, "genres_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");

            entity.HasMany(d => d.Shows).WithMany(p => p.Genres)
                .UsingEntity<Dictionary<string, object>>(
                    "GenreShow",
                    r => r.HasOne<Show>().WithMany()
                        .HasForeignKey("ShowId")
                        .HasConstraintName("genre_shows_show_id_fkey"),
                    l => l.HasOne<Genre>().WithMany()
                        .HasForeignKey("GenreId")
                        .HasConstraintName("genre_shows_genre_id_fkey"),
                    j =>
                    {
                        j.HasKey("GenreId", "ShowId").HasName("genre_shows_pkey");
                        j.ToTable("genre_shows", "episodes");
                        j.HasIndex(new[] { "ShowId" }, "genre_shows_show_id_idx");
                        j.IndexerProperty<int>("GenreId").HasColumnName("genre_id");
                        j.IndexerProperty<int>("ShowId").HasColumnName("show_id");
                    });
        });

        modelBuilder.Entity<Season>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("seasons_pkey");

            entity.ToTable("seasons", "episodes");

            entity.HasIndex(e => e.ShowId, "seasons_show_id_idx");

            entity.HasIndex(e => new { e.ShowId, e.SeasonNumber }, "seasons_show_id_season_number_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AirDate).HasColumnName("air_date");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.DataProviderId).HasColumnName("data_provider_id");
            entity.Property(e => e.ExternalId).HasColumnName("external_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Overview).HasColumnName("overview");
            entity.Property(e => e.PosterImgLink).HasColumnName("poster_img_link");
            entity.Property(e => e.SeasonNumber).HasColumnName("season_number");
            entity.Property(e => e.ShowId).HasColumnName("show_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.DataProvider).WithMany(p => p.Seasons)
                .HasForeignKey(d => d.DataProviderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("seasons_data_provider_id_fkey");

            entity.HasOne(d => d.Show).WithMany(p => p.Seasons)
                .HasForeignKey(d => d.ShowId)
                .HasConstraintName("seasons_show_id_fkey");
        });

        modelBuilder.Entity<Show>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("shows_pkey");

            entity.ToTable("shows", "episodes");

            entity.HasIndex(e => new { e.ExternalId, e.DataProviderId }, "shows_external_id_data_provider_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.DataProviderId).HasColumnName("data_provider_id");
            entity.Property(e => e.EndedDate).HasColumnName("ended_date");
            entity.Property(e => e.ExternalId).HasColumnName("external_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.PosterImgLink).HasColumnName("poster_img_link");
            entity.Property(e => e.PremieredDate).HasColumnName("premiered_date");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.DataProvider).WithMany(p => p.Shows)
                .HasForeignKey(d => d.DataProviderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("shows_data_provider_id_fkey");
        });

        modelBuilder.Entity<TvDataProvider>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tv_data_providers_pkey");

            entity.ToTable("tv_data_providers", "episodes");

            entity.HasIndex(e => e.Name, "tv_data_providers_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<TvNetwork>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tv_networks_pkey");

            entity.ToTable("tv_networks", "episodes");

            entity.HasIndex(e => e.Name, "tv_networks_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.LogoImgLink).HasColumnName("logo_img_link");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");

            entity.HasMany(d => d.Shows).WithMany(p => p.Networks)
                .UsingEntity<Dictionary<string, object>>(
                    "NetworkShow",
                    r => r.HasOne<Show>().WithMany()
                        .HasForeignKey("ShowId")
                        .HasConstraintName("network_shows_show_id_fkey"),
                    l => l.HasOne<TvNetwork>().WithMany()
                        .HasForeignKey("NetworkId")
                        .HasConstraintName("network_shows_network_id_fkey"),
                    j =>
                    {
                        j.HasKey("NetworkId", "ShowId").HasName("network_shows_pkey");
                        j.ToTable("network_shows", "episodes");
                        j.HasIndex(new[] { "ShowId" }, "network_shows_show_id_idx");
                        j.IndexerProperty<int>("NetworkId").HasColumnName("network_id");
                        j.IndexerProperty<int>("ShowId").HasColumnName("show_id");
                    });
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users", "episodes");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(254)
                .HasColumnName("email");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<UserShow>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.ShowId }).HasName("user_shows_pkey");

            entity.ToTable("user_shows", "episodes");

            entity.HasIndex(e => e.ShowId, "user_shows_show_id_idx");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.ShowId).HasColumnName("show_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Show).WithMany(p => p.UserShows)
                .HasForeignKey(d => d.ShowId)
                .HasConstraintName("user_shows_show_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserShows)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_shows_user_id_fkey");
        });

        modelBuilder.Entity<WatchedEpisode>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.EpisodeId }).HasName("watched_episodes_pkey");

            entity.ToTable("watched_episodes", "episodes");

            entity.HasIndex(e => e.EpisodeId, "watched_episodes_episode_id_idx");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.EpisodeId).HasColumnName("episode_id");
            entity.Property(e => e.WatchedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("watched_at");

            entity.HasOne(d => d.Episode).WithMany(p => p.WatchedEpisodes)
                .HasForeignKey(d => d.EpisodeId)
                .HasConstraintName("watched_episodes_episode_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.WatchedEpisodes)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("watched_episodes_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
