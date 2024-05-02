import useRecords from '../../../hooks/useRecords';
import artistService from '../../../hooks/ArtistService';
import genreService from '../../../hooks/GenreService';

import { useState, useEffect } from 'react';

import {
  useToast,
  Box,
  Container,
  Heading,
  FormControl,
  FormLabel,
  Input,
  Textarea,
  Button,
  Flex,
  Badge,
  InputGroup,
  InputLeftElement,
} from '@chakra-ui/react';

import { AsyncSelect, Select } from 'chakra-react-select';

import { FaSearch } from 'react-icons/fa';
import {
  AutoComplete,
  AutoCompleteInput,
  AutoCompleteItem,
  AutoCompleteList,
  AutoCompleteTag,
  AutoCompleteCreatable,
} from '@choc-ui/chakra-autocomplete';

const fetchArtists = async (searchArtistTerm) => {
  try {
    const artists = await artistService.searchArtists(searchArtistTerm);
    const artistsOptions = artists.results.map((artist) => ({
      value: artist.id,
      label: artist.name,
    }));
    return artistsOptions;
  } catch (error) {
    console.error(error);
  }
};

const CreateRecord = () => {
  const toast = useToast();

  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [releaseDate, setReleaseDate] = useState('');
  const [artistIds, setArtistIds] = useState([]);
  const [genreNames, setGenreNames] = useState([]);

  const [searchArtistTerm, setSearchArtistTerm] = useState('');
  const [isArtistLoading, setIsArtistLoading] = useState(true);
  const [artists, setArtists] = useState([]);
  const [searchGenreTerm, setSearchGenreTerm] = useState('');
  const [isGenreLoading, setIsGenreLoading] = useState(true);
  const [genres, setGenres] = useState([]);

  const [selectedArtists, setSelectedArtists] = useState([]);

  useEffect(() => {
    const fetchGenres = async () => {
      try {
        const genres = await genreService.searchGenres(searchGenreTerm);
        setGenres(genres);
      } catch (error) {
        console.error(error);
      }
    };

    fetchGenres();
  }, [searchGenreTerm]);

  useEffect(() => {
    console.log(releaseDate);
  }, [releaseDate]);
  const handleGenreChange = (value) => {
    console.log(value);
    setGenreNames(value);
  };

  const handleArtistChange = (value) => {
    console.log(value);
    setSelectedArtists(value);
    const artistIds = value.map((artist) => artist.value);
    console.log(artistIds);
    setArtistIds(artistIds);
  };

  const { createRecord } = useRecords();

  const handleCreateRecord = async () => {
    try {
      const record = {
        title,
        description,
        releaseDate,
        artistIds,
        genreNames,
      };

      await createRecord(record);

      toast({
        title: 'Запис створено',
        status: 'success',
        duration: 5000,
        isClosable: true,
      });

      setTitle('');
      setDescription('');
      setReleaseDate('');
      setArtistIds([]);
      setGenreNames([]);
    } catch (error) {
      toast({
        title: 'Помилка',
        description: 'Не вдалося створити запис. ' + error.message,
        status: 'error',
        duration: 5000,
        isClosable: true,
      });
    }
  };
  return (
    <Box bg='gray.100' py={12} flexGrow={1}>
      <Container maxW='7xl'>
        <Heading color='teal.600' mb={4}>
          Додати запис
        </Heading>
        <Flex mb={4}>
          <FormControl flex='1' mr={4}>
            <FormLabel>Назва</FormLabel>
            <Input value={title} onChange={(e) => setTitle(e.target.value)} />
          </FormControl>
        </Flex>
        <FormControl mb={4}>
          <FormLabel>Опис</FormLabel>
          <Textarea
            value={description}
            onChange={(e) => setDescription(e.target.value)}
          />
        </FormControl>
        <FormControl mb={4}>
          <FormLabel>Дата виходу</FormLabel>
          <Input
            type='date'
            value={releaseDate}
            onChange={(e) => setReleaseDate(e.target.value)}
          />
        </FormControl>
        {/* <FormControl mb={4}>
          <FormLabel>Виконавець</FormLabel>
          <AutoComplete
            openOnFocus
            mb={4}
            multiple
            onChange={handleArtistChange}
          >
            <AutoCompleteInput
              variant='filled'
              onChange={(e) => setSearchArtistTerm(e.target.value)}
            >
              {({ tags }) =>
                tags.map((tag, tid) => (
                  <AutoCompleteTag
                    key={tid}
                    label={tag.label}
                    onRemove={tag.onRemove}
                    backgroundColor='gray.300'
                  />
                ))
              }
            </AutoCompleteInput>
            <AutoCompleteList>
              {console.log(artists)}
              {artists.map((artist) => (
                <AutoCompleteItem key={artist.id} value={artist}>
                  {artist.name}
                </AutoCompleteItem>
              ))}
            </AutoCompleteList>
          </AutoComplete>
        </FormControl> */}
        <FormControl mb={4}>
          <FormLabel>Виконавець</FormLabel>
          <AsyncSelect
            loadOptions={fetchArtists}
            isMulti
            placeholder='Виберіть виконавців'
            onChange={handleArtistChange}
            value={selectedArtists}
          ></AsyncSelect>
        </FormControl>
        <FormControl mb={4}>
          <FormLabel>Жанри</FormLabel>
          <AutoComplete
            openOnFocus
            mb={4}
            multiple
            creatable
            onChange={handleGenreChange}
          >
            <AutoCompleteInput
              variant='filled'
              onChange={(e) => setSearchGenreTerm(e.target.value)}
            >
              {({ tags }) =>
                tags.map((tag, tid) => (
                  <AutoCompleteTag
                    key={tid}
                    label={tag.label}
                    onRemove={tag.onRemove}
                    backgroundColor='gray.300'
                  />
                ))
              }
            </AutoCompleteInput>
            <AutoCompleteList>
              {genres.map((genre) => (
                <AutoCompleteItem key={genre.name} value={genre.name}>
                  {genre.name}
                </AutoCompleteItem>
              ))}
            </AutoCompleteList>
            <AutoCompleteCreatable />
          </AutoComplete>
        </FormControl>
        <Button
          colorScheme='teal'
          mt={4}
          onClick={handleCreateRecord}
          _hover={{ bg: 'teal.700' }}
          transition='background-color 0.3s ease'
        >
          Створити запис
        </Button>
      </Container>
    </Box>
  );
};

export default CreateRecord;
